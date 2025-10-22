# 🛡️ Security.API
Security.API is a secure and modular ASP.NET Core web service that manages users and clients in a multi-application environment. It exposes REST endpoints for creating, updating, and managing users and clients, enforces API-key authentication, and integrates with a SQL database that can be automatically initialized using the accompanying Security.Migrator console tool.

---

## 📦 Project Overview

Components
• Security.API – Main REST API for managing users, clients, and credentials.
• Security.DataServices – Contains business logic and service layer implementations.
• Security.Repositories – Data-access layer using Entity Framework Core.
• Security.Domain – Entity and DTO model definitions.
• Security.Data.EF – Contains Entity Framework Core context, database configurations, and migrations.
• Security.Migrator – Console application used to initialize and migrate the database schema before starting the API.
• Security.Tests – MSTest project providing unit and integration tests for all layers.

---

## 🚀 Getting Started

### 1️⃣ Prerequisites

Before running Security.API, ensure that:
• You have .NET 9 SDK installed.
• A SQL Server instance is available (local or remote).
• The database schema has been created using the Security.Migrator tool.

The Security.Migrator console application includes complete instructions for usage. It handles database creation and migrations automatically based on your configuration.
documentation README-MIGRATOR.md
---

### 2️⃣ Database Initialization

Run the Security.Migrator tool once before starting the API:

```
cd Security.Migrator/bin/Debug/net9.0
```

This will:
• Create the database if missing.
• Apply the latest migrations.
• Initialize the required system tables.

---

### 3️⃣ Starting the API

After the database is created, start Security.API in one of the following ways:

**Option A – Using Visual Studio or Rider**
Run the project in Development mode.
Swagger UI will open automatically at:
[http://localhost:5200/swagger](http://localhost:5200/swagger)

**Option B – Using the compiled executable**
Navigate to:
Security.API\bin\Debug\net9.0
and run:

```
Security.API.exe "http://localhost:5200"
```

By default, the API listens on [http://localhost:5200](http://localhost:5200).

---

### 🔐 API Key Regeneration

When the Security.API application starts for the first time, it automatically creates a **new API key** and an associated **Admin Client** entry in the database’s `Client` table.
The generated key is stored in the following location:

```
<ExecutableDirectory>\security\apikey.txt
```

If the API key file is accidentally deleted or lost, follow these steps to regenerate it:

1. Open the database and **delete the Admin Client** record from the `Client` table.
2. Restart the **Security.API** application.
3. A **new Admin Client** will be created automatically, and a **new API key** will be generated in the same file location.

This ensures that your system remains secure and that only the new API key is valid for authentication.


---

## 🧭 Exposed API Endpoints

### 👤 User Management API

The **UserController** provides operations for managing system users.

| Method   | Endpoint                           | Description                       | Response Codes                                      |
| -------- | ---------------------------------- | --------------------------------- | --------------------------------------------------- |
| `POST`   | `/api/user`                        | Creates a new user in the system. | 200 (Success), 400 (Invalid input)                  |
| `GET`    | `/api/user/{id}`                   | Retrieves a user by ID.           | 200 (Success), 400 (Invalid input), 404 (Not found) |
| `PUT`    | `/api/user/{id}`                   | Updates an existing user.         | 200 (Success), 400 (Invalid input), 404 (Not found) |
| `DELETE` | `/api/user/{id}`                   | Deletes a user from the system.   | 200 (Success), 400 (Invalid input), 404 (Not found) |
| `POST`   | `/api/user/{id}/validate-password` | Validates a user’s password.      | 200 (Success), 400 (Invalid input), 404 (Not found) |

Each response is returned as a `DataResponse<T>` object with a consistent structure.

Example success response:

```
{
  "responseCode": "Success",
  "data": {
    "id": "c3b8a2e1-234f-4f2a-a7b4-8b5f8a5c9c5a",
    "userName": "john.doe",
    "email": "john.doe@domain.com"
  },
  "succeeded": true,
  "correlationId": "3e902d4d-b4f1-441c-96c0-bbe911e238b0"
}
```

---

### 🧩 Client Management API

The **ClientController** provides operations for managing clients and their activation status.

| Method  | Endpoint                      | Description                         | Response Codes                                      |
| ------- | ----------------------------- | ----------------------------------- | --------------------------------------------------- |
| `POST`  | `/api/client/create`          | Creates a new client in the system. | 200 (Success), 400 (Invalid input)                  |
| `PATCH` | `/api/client/{id}/activate`   | Activates a client by ID.           | 200 (Success), 400 (Invalid input), 404 (Not found) |
| `PATCH` | `/api/client/{id}/deactivate` | Deactivates a client by ID.         | 200 (Success), 400 (Invalid input), 404 (Not found) |

Example success response:

```
{
  "responseCode": "Success",
  "data": true,
  "succeeded": true,
  "correlationId": "f82b14b3-15b7-44c9-b934-5db3df66e8ef"
}
```

---

## 🧱 Exception Handling

All exceptions are handled by the **GlobalExceptionMiddleware**, which ensures:
• Unified JSON responses using the DataResponse<object> model.
• HTTP status mapping (400, 404, 409, 500).
• Correlation ID tracking for traceability.
• Separate logging for known vs unknown exceptions.

Example error response:

```
{
  "responseCode": "NotFound",
  "errorMessage": "User not found.",
  "correlationId": "f6712a0a-3e29-4b51-a46c-b2c5b924af6b"
}
```

---

## 🧪 Testing

The solution includes complete **unit** and **integration** tests:
• Repository layer tested with in-memory EF context.
• Service layer tested with mocks (Moq).
• Controller layer tested using dependency-injected test base.
• Middleware tested using simulated HttpContext pipeline.

To run all tests:

```
dotnet test Security.Tests
```

---

## ⚙️ Configuration

Configuration files define environment and hosting parameters.

Important settings (found in `appsettings.json`):
• ConnectionStrings – Database connection.
• Logging – Log levels and outputs.

Example `appsettings.json`:

```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SecurityDB;Trusted_Connection=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://localhost:5200"
      }
    }
  }
}
```

---

## 🧰 Technologies Used

• .NET 9 / ASP.NET Core
• Entity Framework Core
• MSTest and Moq
• Serilog
• AutoMapper
• Swagger / OpenAPI

---

## 🧾 License

This project is distributed under the **MIT License**.
You are free to modify, reuse, and integrate it into your own systems.

---

## 👨‍💻 Maintainer

**Riste Jovanov** – Lead Developer
Architected and implemented the Security Platform API, database migrator, and automated testing infrastructure.

---

## 🟢 Note

Before first use, always run the **Security.Migrator** tool to initialize the database.
The generated API key, located under the `security` folder, must be included in every request header (`X-API-KEY`) for successful authentication.
