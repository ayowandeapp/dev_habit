using DevHabit.APi.Data;
using DevHabit.APi.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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
    app.MapOpenApi();
    //apply migrations
    await app.ApplyMigrationsAsync();
}

app.UseHttpsRedirection();

// app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
