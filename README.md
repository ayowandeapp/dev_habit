![.NET](https://img.shields.io/badge/-.NET%209.0-blueviolet)
![MySQL](https://img.shields.io/badge/MySQL-F29111)
![OpenAPI](https://img.shields.io/badge/-Swagger-85EA2D?style=flat&logo=swagger&logoColor=white)
![OpenAPI](https://img.shields.io/badge/-Scalar-0F0F0F?style=flat&logo=swagger&logoColor=white)

# DevHabit API

## Table of Contents

<ol>
  <li>
    <a href="#-overview">Overview</a>
  </li>
  <li>
    <a href="#-technologies">Technologies</a>
  </li>
  <li>
    <a href="#-features">Features</a>
  </li>
  <li>
    <a href="#-getting-started">Getting Started</a>
    <ul>
      <li><a href="#-prerequisites">Prerequisites</a></li>
      <li><a href="#-installation">Installation</a></li>
    </ul>
  </li>
  <li>
    <a href="#-environment-setup">Environment Setup</a>
    <ul>
      <li><a href="#-local-development">Local Development</a></li>
    </ul>
  </li>
  <li>
    <a href="#-testing">Testing</a>
  </li>
  <li>
    <a href="#-api-documentation">API Documentation</a>
  </li>
</ol>

## 🎯 Overview

DevHabit API is a versioned RESTful API developed with .NET 9, designed to help users monitor and improve their personal habits and routines. The service features:

> Secure JWT-based authentication
> Background job processing for asynchronous tasks
> GitHub integration for seamless workflow connectivity
> Structured observability via OpenTelemetry for monitoring and logging

Built with MySQL, the API follows industry best practices in validation, testing, and deployment. It supports:
✔ Local development setups
✔ CI/CD pipelines for automated workflows (pending)

This project was developed as part of Milan Jovanović's course, Pragmatic REST APIs, leveraging the latest ASP.NET Core features and API design best practices.

## 🔧 Technologies

- [.NET 9](https://dotnet.microsoft.com/) – Modern framework for building scalable web APIs
- [MySQL](https://www.mysql.org/) – Open-source relational database
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) – ORM for data access
- [Swagger (Swashbuckle)](https://swagger.io/) – OpenAPI interactive documentation
- [Scalar](https://scalar.com/) – Alternative interactive API explorer
- [FluentValidation](https://docs.fluentvalidation.net/) – Model validation framework
- [Quartz.NET](https://www.quartz-scheduler.net/) – Background jobs and scheduling
- [Polly (via Microsoft.Extensions.Http.Resilience)](https://www.pollydocs.org/) – Fault-handling and resiliency
- [Refit](https://reactiveui.github.io/refit/) – Typed REST API clients
- [CsvHelper](https://joshclose.github.io/CsvHelper/) – CSV import/export utilities
- [OpenTelemetry](https://opentelemetry.io/) – Distributed tracing and observability
- [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/) – Simplified distributed application model with OpenTelemetry support
- [xUnit](https://xunit.net/) – Unit testing framework
- [NSubstitute](https://nsubstitute.github.io/) – Friendly mocking for .NET

## ✨ Features

#### 🧭 API Design
- Filtering, Sorting & Pagination – Efficiently manage large datasets with customizable queries.
- Data Shaping – Retrieve only the required fields to optimize payload size.
- Content Negotiation – Supports multiple response formats (JSON, XML) for client flexibility.
- HATEOAS (Hypermedia as the Engine of Application State) – Enables discoverable, self-documenting API interactions.
- API Versioning – Seamless version control via media type (Accept header) for backward compatibility
- Typed HTTP Clients (Refit) – Strongly-typed REST client integration for cleaner API consumption.
- Interactive API Documentation – Auto-generated docs with Swagger UI and enhanced readability via Scalar.

#### ⚙️ Infrastructure & Integration
- MySQL with EF Core and naming conventions
- Background jobs with Quartz
- GitHub integration for external data or automation
- File-based data import (CSV support)

#### 🔐 Security
- JWT-based authentication and authorization
- CORS policy configuration
- secure storage of GitHub api keys via encryption

#### 🧪 Testing Stack
- Unit testing with xUnit and NSubstitute

#### 📦 Deployment & DevOps
- Local development support

## 🚀 Getting Started

### 📋 Prerequisites

Make sure you have .NET CLI installed on your system. You can check if it's available by running:

```bash
dotnet --version
```

This should print the installed version of the .NET CLI. If it's not installed, download it from the [official .NET site](https://dotnet.microsoft.com/download).

To verify which SDK versions are installed:

```bash
dotnet --list-sdks
```

> [!IMPORTANT]
> The minimum .NET SDK version required is **9.0.0**

Additionally, the project uses Mysql for running database service.

---

### 📥 Installation

To get started, clone the repository and set up the environment configuration:

1. Clone the repository:

```bash
git clone https://github.com/jaimejaramilloperez/dev-habit.git
```

2. Navigate to the project directory:

```bash
cd dev-habit
```

3. Generate and trust the HTTPS development certificate:

```bash
dotnet dev-certs https -ep ./src/DevHabit.Api/aspnetapp.pfx -p Test1234!
dotnet dev-certs https --trust
```

```

After installation, you're ready to run the app either locally . See the [Local Development](#-local-development) sections for details.

## 💻 Environment SetUp

Set up your environment to run the DevHabit API either locally, depending on your workflow.

> [!NOTE]
> The configuration values shown (e.g., passwords, ports, keys, connection strings) are provided for demonstration purposes only. You are free to modify them as needed — especially for production environments.

---

### 🧑‍💻 Local Development

You can run the API locally using the .NET CLI and supporting services (MySQL, phpmyAdmin,) .

1. Configure user secrets:

Sensitive values should be stored securely using [user secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-9.0&tabs=linux):

```json
{
  "ConnectionStrings:Database": "Server=localhost;Port=5432;Database=devhabit;Username=devhabit;Password=123456;",
  "Jwt:Key": "HTycXOjdDRfrtNYzQQbkx2L7ncCEe2989cWH6yrTFdSPRmFFe4K9qmbnjHJBRGHfaeRKvDEWzaS",
  "Encryption:Key": "Ubf/RatKuzJ4p8Fc9nr9LKZFV5L8CjIZZCqcFlYZeEo="
}
```

2. Update `appsettings.Development.json`

```json
{
  "ConnectionStrings": {
    "Database": "your-database-connection-string-here"
  },
  "Jwt": {
    "Key": "your-secret-key-here-that-should-also-be-fairly-long",
    "Issuer": "dev-habit.api",
    "Audience": "dev-habit.app",
    "ExpirationInMinutes": 30,
    "RefreshTokenExpirationInDays": 7
  },
  "Encryption": {
    "Key": "your-secret-key-here-that-should-also-be-exactly-32-bytes-or-44-characters-in-base64-long"
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000"
    ]
  },
  "Jobs": {
    "ScanIntervalInMinutes": 50
  },
  "GitHub": {
    "BaseUrl": "https://api.github.com"
  },
  "OTEL_EXPORTER_OTLP_ENDPOINT": "http://localhost:18889",
  "OTEL_EXPORTER_OTLP_PROTOCOL": "grpc",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Information"
    }
  }
}
```

3. Run the API:

```bash
dotnet run --project DevHabit.Api
# or with HTTPS
dotnet run --launch-profile https --project s/DevHabit.Api
```

---

## 🧪 Testing

This project includes unit, integration and functional testing.

> [!NOTE]
> Tests are located in the `test/` directory.

#### Testing Technologies:

- **xUnit**: Test framework.
- **NSubstitute**: Mocking.

#### Running all tests

```bash
dotnet test
```

## 📘 Api Documentation

DevHabit API provides interactive documentation via Swagger and Scalar, with support for versioned endpoints and JWT authentication.

Once the API is running:

- **OpenAPI spec (JSON)**: `https://localhost:5001/swagger/1.0/swagger.json`
- **Swagger UI**: `https://localhost:5001/swagger`

> [!NOTE]
> Replace `5001` with your actual HTTPS port if different.
