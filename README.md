# LABHTTP

# LABHTTP – HTTP & JWT Authentication with ASP.NET Core

This project is a **Lab 10 implementation** of HTTP concepts using **C# and ASP.NET Core Web API**.
It mirrors the ideas shown in Java/Spring Boot (controllers, services, repositories, security), but is implemented using the .NET ecosystem.

The application demonstrates:

* HTTP request → response flow
* REST endpoints
* Database persistence with Entity Framework Core
* User registration and login
* JWT authentication stored in **HTTP-only cookies**

---

## Tech Stack

* **.NET 9 / ASP.NET Core Web API**
* **Entity Framework Core**
* **PostgreSQL** (via Npgsql)
* **JWT (JSON Web Tokens)**
* **Cookie-based authentication**
* **CORS configuration**

---

## Project Structure

```
LABHTTP
├─ Controllers
|  └─ HelloController.cs
│  └─ UserController.cs
├─ Data
|  └─ AppDbContext.cs
│  └─ User.cs
├─ Services
│  └─ PasswordHasher.cs
│  └─ UserService.cs
├─ Repository
│  ├─ IUserRepository.cs
│  └─ UserRepository.cs
├─ Model
│  ├─ User.cs
│  └─ DTO
│     ├─ LoginRequest.cs
│     └─ CreateUserRequest.cs
├─ Program.cs
└─ README.md
```

**Responsibilities:**

* Controllers → HTTP layer
* Services → business logic (JWT generation)
* Repositories → database access
* Models → domain & DTOs

---

## Environment Variables

Created a env.variables and used them in code, while their value located on PC

```
DB_CONNECTION
JWT_SECRET
```

---

## Database Setup

1. Make sure PostgreSQL is running
2. Create the database:

```sql
CREATE DATABASE labhttp;
```

3. Apply Entity Framework migrations:

```bash
dotnet ef database update
```

This will create the `Users` table.

---

## Running the Application

```bash
dotnet run
```

The API will start on:

```
http://localhost:5099
https://localhost:7048
```
---

## Available Endpoints

### Test Endpoint

```
GET /hello
```

Response:

```
200 OK
Hello, user!
```

---

### Register User

```
POST /api/user/register
```

Request body:

```json
{
  "email": "test@example.com",
  "password": "password123"
}
```

Response:

* `200 OK`
* JWT token is set in **HTTP-only cookie** (`jwt`)

---

### Login User

```
POST /api/user/login
```

Request body:

```json
{
  "email": "test@example.com",
  "password": "password123"
}
```

Response:

* `200 OK`
* JWT token is set in **HTTP-only cookie** (`jwt`)

---

## JWT & Cookies

* JWT is generated on login/register
* Token is stored in a **HttpOnly cookie**
* Cookie settings (local development):

  * `HttpOnly = true`
  * `Secure = true`
  * `SameSite = SameSiteMode.None`

For production:

* Use `HTTPS`
* Set `Secure = true`
* Use `SameSite = None` if cross-origin

---

## CORS Configuration

If calling the API from a frontend (React, etc.):

* Cookies require:

  ```js
  credentials: "include"
  ```

* Backend must allow credentials and specify origin
