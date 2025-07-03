using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using DevHabit.APi.Data;
using DevHabit.APi.DTOs.Common;
using DevHabit.APi.DTOs.Habits;
using DevHabit.APi.Middleware;
using DevHabit.APi.Models;
using DevHabit.APi.Services;
using DevHabit.APi.Services.Sorting;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

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
            builder.Services.AddTransient<SortMappingProvider>();

            builder.Services.AddTransient(typeof(IDataShaperService<>), typeof(DataShaperService<>));

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<LinkService>();

            return builder;
        }

        public static WebApplicationBuilder AddAuthenticationServices(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>();

            return builder;
        }
    }
}