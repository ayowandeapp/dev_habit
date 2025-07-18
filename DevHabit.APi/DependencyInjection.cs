using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Asp.Versioning;
using DevHabit.APi.Data;
using DevHabit.APi.DTOs.Common;
using DevHabit.APi.DTOs.Entries;
using DevHabit.APi.DTOs.Habits;
using DevHabit.APi.Jobs;
using DevHabit.APi.Middleware;
using DevHabit.APi.Models;
using DevHabit.APi.Services;
using DevHabit.APi.Services.Sorting;
using DevHabit.APi.Settings;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Quartz;

namespace DevHabit.APi
{
    public static class DependencyInjection
    {
        public static WebApplicationBuilder AddController(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers(options =>
                {
                    options.ReturnHttpNotAcceptable = true;

                })
                .AddXmlSerializerFormatters();

            // Add media type to supported types
            builder.Services.Configure<MvcOptions>(options =>
            {
                var jsonFormatter = options.OutputFormatters
                    .OfType<SystemTextJsonOutputFormatter>()
                    .First();

                jsonFormatter.SupportedMediaTypes.Add(
                    CustomMediaTypeNames.Application.HateoasJson);
                jsonFormatter.SupportedMediaTypes.Add(
                    CustomMediaTypeNames.Application.HateoasJsonV1);
                jsonFormatter.SupportedMediaTypes.Add(
                    CustomMediaTypeNames.Application.HateoasJsonV2);
                jsonFormatter.SupportedMediaTypes.Add(
                    CustomMediaTypeNames.Application.JsonV1);
                jsonFormatter.SupportedMediaTypes.Add(
                    CustomMediaTypeNames.Application.JsonV2);
            });

            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1.0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionSelector = new DefaultApiVersionSelector(options);

                options.ApiVersionReader = ApiVersionReader.Combine(
                    new MediaTypeApiVersionReader(),
                    new MediaTypeApiVersionReaderBuilder()
                        .Template("application/vnd.dev-habit.hateoas.{version}+json")
                        .Build()
                );

            })
            .AddMvc();

            builder.Services.AddOpenApi();

            return builder;

        }

        public static WebApplicationBuilder AddErrorHandling(this WebApplicationBuilder builder)
        {
            builder.Services.AddProblemDetails();
            builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            return builder;

        }

        public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(options =>
                options
                    .UseSnakeCaseNamingConvention()
                    .UseMySql(
                        builder.Configuration.GetConnectionString("DefaultConnection"),
                        new MariaDbServerVersion(new Version(10, 4, 28))
                    )
            );

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
                options
                    .UseSnakeCaseNamingConvention()
                    .UseMySql(
                        builder.Configuration.GetConnectionString("DefaultConnection"),
                        new MariaDbServerVersion(new Version(10, 4, 28))
                    )
            );

            return builder;
        }

        public static WebApplicationBuilder AddAplicationServices(this WebApplicationBuilder builder)
        {
            // builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            //register sorting definitions
            builder.Services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<HabitDto, Habit>>(_ =>
                HabitMappers.sortMapping);
            builder.Services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<EntryDto, Entry>>(_ =>
                EntryMappers.sortMapping);
            builder.Services.AddTransient<SortMappingProvider>();
            builder.Services.AddTransient<TokenProvider>();

            builder.Services.AddTransient(typeof(IDataShaperService<>), typeof(DataShaperService<>));

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<LinkService>();

            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<UserContext>();

            builder.Services.AddScoped<GitHubAccessTokenService>();
            builder.Services.AddTransient<GitHubService>();
            builder.Services
                .AddHttpClient("github")
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri("https://api.github.com");
                    client.DefaultRequestHeaders
                        .UserAgent.Add(new ProductInfoHeaderValue("DevHabit", "1.0"));

                    client.DefaultRequestHeaders
                        .Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
                });

            builder.Services.Configure<EncryptionOptions>(builder.Configuration.GetSection("Encryption"));
            builder.Services.AddTransient<EncryptionService>();

            return builder;
        }

        public static WebApplicationBuilder AddAuthenticationServices(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>();
            //inject the JwtAuthOptions
            builder.Services.Configure<JwtAuthOptions>(builder.Configuration.GetSection("Jwt"));

            JwtAuthOptions jwtAuthOptions = builder.Configuration.GetSection("Jwt").Get<JwtAuthOptions>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtAuthOptions.Issuer,
                        ValidAudience = jwtAuthOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuthOptions.Key))
                    };
                });

            builder.Services.AddAuthorization();

            return builder;
        }
        public static WebApplicationBuilder AddBackgroundjobs(this WebApplicationBuilder builder)
        {
            builder.Services.AddQuartz(q =>
            {
                // Base job configuration
                q.SchedulerId = "HabitTracker-Scheduler";
                q.SchedulerName = "Habit Tracker Job Scheduler";

                q.AddJob<GitHubAutomationSchedularJob>(j => j
                    .WithIdentity("github-auomation-schedular")
                );
    
                q.AddTrigger(t => t
                    .ForJob("github-auomation-schedular")
                    .WithIdentity("github-auomation-schedular-trigger")
                    .WithSimpleSchedule(s =>
                    {
                        s.WithIntervalInMinutes(1).RepeatForever();
                    })                    
                );


            });
            // Add Quartz hosted service
            builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            return builder;
        }
    }
}