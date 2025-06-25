using DevHabit.APi.Data;
using DevHabit.APi.DTOs.Habits;
using DevHabit.APi.Extensions;
using DevHabit.APi.Middleware;
using DevHabit.APi.Models;
using DevHabit.APi.Services.Sorting;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = false;///
})
.AddXmlSerializerFormatters();

// builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

//register sorting definitions
builder.Services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<HabitDto, Habit>>( _ => 
    HabitMappers.sortMapping);
builder.Services.AddTransient<SortMappingProvider>();

builder.Services.AddOpenApi();

//Dependencies
builder.Services.AddDbContext<AppDbContext>(options =>
    options
        .UseSnakeCaseNamingConvention()
        .UseMySql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            new MariaDbServerVersion(new Version(10, 4, 28))
        )
);

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapOpenApi();
    //apply migrations
    await app.ApplyMigrationsAsync();
}

app.UseHttpsRedirection();

// app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

await app.RunAsync();
