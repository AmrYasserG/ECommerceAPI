# ECommerce REST API

A fully-featured backend REST API for an e-commerce platform built with **ASP.NET Core (.NET 10)** following clean N-Tier Architecture principles.

## Features

- JWT Authentication & Authorization
- Role-Based Access Control (Admin / User)
- Product browsing with filtering, search, and pagination
- Shopping cart management
- Order processing
- Image upload and serving
- Full CRUD for Products and Categories

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET Core Web API (.NET 10) |
| Database | SQL Server + Entity Framework Core 10 |
| Authentication | ASP.NET Core Identity + JWT Bearer |
| Architecture | N-Tier (PL / BLL / DAL / Common) |
| ORM Pattern | Repository Pattern + Unit of Work |
| Validation | FluentValidation |
| Mapping | AutoMapper |
| API Docs | Scalar (OpenAPI) |

---

## Project Structure

```
ECommerceAPI/
│
├── ECommerceAPI/          # Presentation Layer (Controllers, Program.cs)
├── ECommerceAPI.BL/       # Business Logic Layer (Managers, DTOs, Validators, Mapping)
├── ECommerceAPI.DAL/      # Data Access Layer (DbContext, Repositories, UnitOfWork)
└── ECommerceAPI.Common/   # Shared Models, Enums, Response Wrapper
```

---

## NuGet Packages

### ECommerceAPI.DAL
| Package | Version |
|---------|---------|
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 10.0.8 |
| Microsoft.EntityFrameworkCore.SqlServer | 10.0.8 |
| Microsoft.EntityFrameworkCore.Design | 10.0.8 |

### ECommerceAPI.BL
| Package | Version |
|---------|---------|
| AutoMapper | 12.0.1 |
| FluentValidation | 11.9.2 |
| System.IdentityModel.Tokens.Jwt | 8.0.0 |

### ECommerceAPI (Presentation Layer)
| Package | Version |
|---------|---------|
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.0.1 |
| FluentValidation.AspNetCore | 11.3.0 |
| Microsoft.AspNetCore.Authentication.JwtBearer | 10.0.8 |
| Microsoft.AspNetCore.OpenApi | 10.0.8 |
| Microsoft.EntityFrameworkCore.Design | 10.0.8 |
| Microsoft.EntityFrameworkCore.Tools | 10.0.8 |
| Scalar.AspNetCore | 2.14.14 |

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server or SQL Server Express
- Visual Studio 2022+ or VS Code

### Setup

**1. Clone the repository**
```bash
git clone https://github.com/AmrYasserG/ECommerceAPI.git
cd ECommerceAPI
```

**2. Configure the database connection**

Open `ECommerceAPI/appsettings.json` and update the connection string:
```json
"ConnectionStrings": {
  "DefaultConnection": "YOUR_CONNECTION_STRING"
}
```

**3. Apply migrations**
```bash
dotnet ef database update --project ECommerceAPI.DAL --startup-project ECommerceAPI
```

This will create the database, all tables, and seed:
- 3 Categories (Electronics, Clothing, Books)
- 7 Products
- Admin and User roles
- Default admin account

**4. Run the API**
```bash
dotnet run --project ECommerceAPI
```

Or press **F5** in Visual Studio.

API runs at: `https://localhost:7179`
Scalar UI: `https://localhost:7179/scalar`

---

## Default Admin Account

| Field | Value |
|-------|-------|
| Email | admin@ecommerce.com |
| Password | Admin@123456 |

---

## API Endpoints

### Authentication
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| POST | `/api/auth/register` | Public | Register a new user |
| POST | `/api/auth/login` | Public | Login and get JWT token |

### Categories
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| GET | `/api/categories` | Public | Get all categories |
| GET | `/api/categories/{id}` | Public | Get category by ID |
| POST | `/api/categories` | Admin | Create a category |
| PUT | `/api/categories/{id}` | Admin | Update a category |
| DELETE | `/api/categories/{id}` | Admin | Delete a category |
| POST | `/api/categories/{id}/image` | Admin | Set category image |

### Products
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| GET | `/api/products` | Public | Get products (filter + search + pagination) |
| GET | `/api/products/{id}` | Public | Get product by ID |
| POST | `/api/products` | Admin | Create a product |
| PUT | `/api/products/{id}` | Admin | Update a product |
| DELETE | `/api/products/{id}` | Admin | Delete a product |
| POST | `/api/products/{id}/image` | Admin | Set product image |

**Query Parameters for GET /api/products:**
```
?categoryId=1&name=laptop&pageNumber=1&pageSize=10
```

### Cart
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| GET | `/api/cart` | User | Get current user's cart |
| POST | `/api/cart` | User | Add item to cart |
| PUT | `/api/cart` | User | Update item quantity |
| DELETE | `/api/cart/{productId}` | User | Remove item from cart |

### Orders
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| POST | `/api/orders` | User | Place order from cart |
| GET | `/api/orders` | User | Get order history |
| GET | `/api/orders/{id}` | User | Get order details |

### File Management
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| POST | `/api/image/upload` | User | Upload an image |
| POST | `/api/products/{id}/image` | Admin | Set product image |
| POST | `/api/categories/{id}/image` | Admin | Set category image |

---

## Authentication

This API uses **JWT Bearer tokens**. After login or register, copy the token from the response and include it in subsequent requests:

```
Authorization: Bearer <your_token_here>
```

**Access Levels:**
- **Public** — No token required
- **User** — Any authenticated user (regular or admin)
- **Admin** — Must have the Admin role

---

## API Response Format

All endpoints return a consistent response wrapper:

```json
{
  "success": true,
  "message": "Operation successful.",
  "data": { },
  "errors": null
}
```

---

## Postman Collection

A Postman collection is included in the repository root:

```
ECommerceAPI_Collection.postman_collection.json
```

Import it in Postman to get all endpoints ready to test.

**Steps:**
1. Import the collection file into Postman
2. Call **Login - Admin** and copy the token from the response
3. Paste it into the `Authorization` header of any admin request 
4. Call **Login - User** and do the same for user requests

---

## Image Upload Rules

- Allowed types: `.jpg`, `.jpeg`, `.png`, `.gif`
- Max file size: **5MB**
- Images are served from: `/images/{filename}`

---

## Postman Testing Video

> [Watch the full API testing walkthrough here](https://drive.google.com/file/d/18XnuMnBLsTINPQCZcBeKf64-5rXhekjb/view?usp=sharing)

