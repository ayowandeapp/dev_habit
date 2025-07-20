using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DevHabit.APi.Middleware
{
    public class ETagMiddleware(RequestDelegate next)
    {
        private static readonly string[] ConcurrencyCheckMethods =
        [
            HttpMethods.Patch,
            HttpMethods.Put,
        ];

        public async Task InvokeAsync(HttpContext context, InMemoryETagStore eTagStore)
        {
            if (CanSkipETag(context))
            {
                await next(context);
                return;
            }
            string resourceUri = context.Request.Path.Value!;
            var ifNoneMatch = context.Request.Headers["IfNoneMatch"].FirstOrDefault();
            var ifMatch = context.Request.Headers["IfMatch"].FirstOrDefault();

            if (ConcurrencyCheckMethods.Contains(context.Request.Method) &&
            !string.IsNullOrEmpty(ifMatch)
            )
            {
                string currentETag = eTagStore.GetTag(resourceUri);

                if (!string.IsNullOrWhiteSpace(currentETag) && ifMatch != currentETag)
                {
                    context.Response.StatusCode = StatusCodes.Status412PreconditionFailed;
                    context.Response.ContentLength = 0;
                    return;
                }
                
            }

            var originalStream = context.Response.Body;
            using var ms = new MemoryStream();
            context.Response.Body = ms;

            await next(context);

            if (IsETaggableResponse(context))
            {
                ms.Position = 0;
                // byte[] responseBody = await GetResponseBody(ms);
                string etag = GenerateETag(ms.ToArray());

                eTagStore.SetTag(resourceUri, etag);
                context.Response.Headers.ETag = etag;
                context.Response.Body = originalStream;

                if (context.Request.Method == HttpMethods.Get && ifNoneMatch == etag)
                {
                    context.Response.StatusCode = StatusCodes.Status304NotModified;
                    context.Response.ContentLength = 0;
                    return;
                }

                ms.Position = 0;
                await ms.CopyToAsync(originalStream);
                context.Response.Body = originalStream;
            }
        }

        private static string GenerateETag(byte[] ms)
        {
            var hash = SHA256.HashData(ms);
            var hex = Convert.ToHexStringLower(hash);
            return $"\"{hex}\"";
        }

        private static bool IsETaggableResponse(HttpContext context)
        {
            return context.Response.StatusCode == StatusCodes.Status200OK &&
                (context.Response.Headers.ContentType
                    .FirstOrDefault()?.Contains("json", StringComparison.OrdinalIgnoreCase) ?? false);
        }

        private static bool CanSkipETag(HttpContext context)
        {
            return context.Request.Method == HttpMethods.Post ||
                context.Request.Method == HttpMethods.Delete;
        }
    }

    public sealed class InMemoryETagStore
    {
        private static readonly ConcurrentDictionary<string, string> ETags = new();

        public string GetTag(string resourceUri)
        {
            return ETags.GetOrAdd(resourceUri, _ => string.Empty);
        }

        public void SetTag(string resourceUri, string etag)
        {
            ETags.AddOrUpdate(resourceUri, etag, (_, _) => etag);
        }
        
        public void RemoveETag(string resourceUri)
        {
            ETags.TryRemove(resourceUri, out _);
        }
        
        private static string GenerateETag(object resource)
        {
            var json = JsonSerializer.Serialize(resource);
            var content = Encoding.UTF8.GetBytes(json);
            var hash = SHA256.HashData(content);
            var hex = Convert.ToHexStringLower(hash);
            return $"\"{hex}\"";
        }

        public void SetTag(string resourceUri, object resource)
        {
            ETags.AddOrUpdate(resourceUri, GenerateETag(resource), (_, _) => GenerateETag(resource));
        }
    }
}