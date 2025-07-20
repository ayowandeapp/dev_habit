using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DevHabit.APi.Middleware
{
    public class ETagMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context, InMemoryETagStore eTagStore)
        {
            if (CanSkipETag(context))
            {
                await next(context);
                return;
            }
            string resourceUri = context.Request.Path.Value!;
            var ifNoneMatch = context.Request.Headers["IfNoneMatch"].FirstOrDefault();

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

        private async Task<byte[]> GetResponseBody(MemoryStream ms)
        {
            // using var reader = new Stream
            throw new NotImplementedException();
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
    }
}