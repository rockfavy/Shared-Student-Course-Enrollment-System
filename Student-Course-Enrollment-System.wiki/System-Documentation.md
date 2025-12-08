# Student Course Enrollment System – System Documentation

---

## 1. Overview

The Student Course Enrollment System is a Blazor WebAssembly application that enables students to register, authenticate, browse courses, enroll in courses, and manage their enrollments. The system uses a Blazor WASM frontend communicating with a Minimal API backend, utilizing in-memory data storage for rapid development and testing.

### Main Modules

- **Authentication Module** – Student registration and login  
- **Courses Module** – Course browsing and CRUD operations  
- **Enrollments Module** – Student course enrollment and deregistration management  

### Authentication Overview
- **Local Environment:** JWT Bearer with symmetric key (self-issued tokens)  
- **Production Environment:** Entra ID (Azure AD)  

### Architectural Patterns
- **API**: Vertical slice architecture - each feature is self-contained with endpoints, DTOs, and business logic grouped together
- **Frontend**: Thin layer architecture - minimal business logic, delegates to API, focuses on presentation and user interaction

### Design Principles
The system follows established software engineering principles:
- **SOLID**: Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion
- **KISS**: Keep It Simple, Stupid - prefer straightforward solutions
- **DRY**: Don't Repeat Yourself - extract common functionality
- **YAGNI**: You Aren't Gonna Need It - implement only what's needed

---

## 2. Architecture Diagram

### High-Level System Architecture

::: mermaid
flowchart LR

User[Student] --> WASM[Blazor WASM Frontend]
WASM --> API[Minimal API]
API --> AUTH[Authentication Service]
API --> COURSES[Courses Module]
API --> ENROLL[Enrollments Module]
API --> DB[(In-Memory Database)]
:::

### Legend

- **User** – Student users  
- **WASM** – Blazor WebAssembly UI  
- **API** – Minimal API backend  
- **Modules** – Feature-based functional units (Authentication, Courses, Enrollments)  
- **DB** – In-memory database  

---

## 3. Technology Stack

| Area | Technology | Version | Notes |
|------|------------|---------|-------|
| Frontend | Blazor WebAssembly | .NET 8 | Razor Components |
| API | ASP.NET Core Minimal API | .NET 8 | Feature-based endpoints |
| Database | In-Memory Database | EF Core | Development/testing only |
| Authentication | JWT/OIDC | Latest | Token-based auth |
| HTTP Client | HttpClient | Built-in | API communication |
| Logging | Built-in Logging | .NET 8 | Structured logging |

---

## 4. Runtime Views / Process Flows

### 4.1 Registration & Login Flow

::: mermaid
sequenceDiagram
participant Student
participant WASM as Blazor WASM
participant API as Minimal API
participant AUTH as Auth Service
participant DB as In-Memory DB

Student->>WASM: Navigate to registration
WASM->>API: POST /auth/register
API->>DB: Store student credentials
DB->>API: Return success
API->>WASM: Return auth token
WASM->>Student: Redirect to courses

Student->>WASM: Navigate to login
WASM->>API: POST /auth/login
API->>DB: Validate credentials
DB->>API: Return student data
API->>WASM: Return auth token
WASM->>Student: Grant access
:::

### 4.2 Course Enrollment Flow

::: mermaid
sequenceDiagram  
participant Student  
participant WASM as Blazor WASM  
participant API as Minimal API  
participant DB as In-Memory DB

Student->>WASM: View available courses  
WASM->>API: GET /courses  
API->>DB: Query courses  
DB->>API: Return course list  
API->>WASM: Return courses  
WASM->>Student: Display courses  

Student->>WASM: Click enroll  
WASM->>API: POST /enrollments  
API->>DB: Create enrollment  
DB->>API: Return success  
API->>WASM: Confirm enrollment  
WASM->>Student: Show confirmation  

Student->>WASM: View my enrollments  
WASM->>API: GET /enrollments/me  
API->>DB: Query student enrollments  
DB->>API: Return enrollments  
API->>WASM: Return enrollments  
WASM->>Student: Display enrolled courses  
:::

---

## 5. Environments

| Environment | URL | Purpose | Authentication | Database | Deployment Method |
|-------------|-----|---------|----------------|----------|-------------------|
| Local | localhost | Local development, debugging | JWT Bearer (symmetric key) | In-memory | Manual run |
| Production | (Prod URL) | Live environment | JWT Bearer (Entra ID token issuer) | In-memory | Manual approval required |

---

## 6. Cross-Cutting Concerns

### Authentication & Authorization

- **Local Environment:** JWT Bearer authentication with symmetric key (self-issued tokens)  
- **Production Environment:** JWT Bearer authentication with Entra ID as token issuer  
- Claims and policies for authorization  

### Logging

- Built-in .NET logging for structured logs  
- Console logging for development  
- Configurable for production environments  

### Configuration Management

- `appsettings.json` for configuration  
- Environment-specific settings  
- API URL configuration for frontend  

---


