# Student Task Management System (ASP.NET Core Web API)

A resume-ready backend project built with **C#**, **ASP.NET Core Web API**, **Entity Framework Core**, and **SQL Server**.  
It supports **JWT authentication**, **role-based authorization (Admin/Student)**, and full **task assignment + management** workflows.

---

## Features

### Authentication & Authorization
- JWT-based login
- Role-based access: **Admin** and **Student**
- Secure password hashing (ASP.NET Core Identity PasswordHasher)

### Task Management
- Admin can **create / update / delete (soft delete)** tasks
- Admin can **assign tasks to students**
- Students can **view their tasks** and **update task status**
- Filtering + pagination support

### API Quality
- Swagger/OpenAPI documentation
- Global exception handling middleware
- EF Core migrations + SQL Server database

---

## Tech Stack

- **.NET 8 / C#**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **SQL Server**
- **JWT Authentication**
- **Swagger (Swashbuckle)**

---

## Solution Structure
│
├── StudentTaskManager.Api
│ ├── Controllers
│ ├── Middleware
│ └── Program.cs
│
├── StudentTaskManager.Domain
│ ├── Entities
│ └── Enums
│
├── StudentTaskManager.Application
│ ├── DTOs
│ ├── Interfaces
│ └── Common
│
└── StudentTaskManager.Infrastructure
├── Auth
├── Data
├── Services
└── Migrations

---

## ⚙️ Prerequisites

- Visual Studio 2022 / VS Code
- .NET SDK 8
- SQL Server / SQL Server Express / LocalDB
- SQL Server Management Studio (optional)

---

## 🔧 Configuration

Update connection string in:

`StudentTaskManager.Api/appsettings.json`



## Example (SQLEXPRESS)

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_MACHINE\\SQLEXPRESS;Database=StudentTaskManagerDb;Trusted_Connection=True;TrustServerCertificate=True"
}


---


