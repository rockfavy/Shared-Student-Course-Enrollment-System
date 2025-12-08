# Student Course Enrollment System - Wiki Documentation

This wiki contains comprehensive documentation for the Student Course Enrollment System, a Blazor WebAssembly application with a Minimal API backend.

---

## Wiki Structure

### Business Documentation
- **[Course Enrollment System](./Course-Enrollment-System.md)** - System overview
- **[Business Overview](./Course-Enrollment-System/1.-Business-Overview.md)** - Business purpose and features
- **[Business Processes](./Course-Enrollment-System/2.-Business-Processes.md)** - User stories and workflows
- **[Course Enrollment Management PRD](./Course-Enrollment-System/2.-Business-Processes/2.1-Course-Enrollment-Management.md)** - Detailed product requirements

### System Documentation
- **[System Documentation Overview](./System-Documentation.md)** - High-level system architecture

#### Front-End Documentation (Blazor WASM)
- **[Front-End Overview](./System-Documentation/1.-Front%2DEnd-Documentation.md)** - Frontend architecture overview
- **[Local Development Setup](./System-Documentation/1.-Front%2DEnd-Documentation/1.0-local_development_setup.md)** - Setup instructions
- **[Project Structure](./System-Documentation/1.-Front%2DEnd-Documentation/1.1-Project-Structure.md)** - Folder organization
- **[AI Rules - Project Structure](./System-Documentation/1.-Front%2DEnd-Documentation/1.1-Project-Structure/1_1_project_structure_ai_rules.md)** - Development guidelines

#### API Documentation (Minimal API)
- **[API Overview](./System-Documentation/2.-API-Documentation.md)** - API architecture overview
- **[Local Development Setup](./System-Documentation/2.-API-Documentation/2.0-local_development_setup.md)** - Setup instructions
- **[Project Structure](./System-Documentation/2.-API-Documentation/2.1-Project-Structure.md)** - Feature-based organization
- **[Authentication Configuration](./System-Documentation/2.-API-Documentation/2.7-Authentication-Configuration.md)** - Environment-specific authentication
- **[AI Rules - Project Structure](./System-Documentation/2.-API-Documentation/2.1-Project-Structure/2_1_project_structure_ai_rules.md)** - Development guidelines

#### Database Documentation
- **[Database Overview](./System-Documentation/3.-Database-Documentation.md)** - In-memory database schema and relationships

#### Infrastructure Documentation
- **[Infrastructure Overview](./System-Documentation/4.-Infrastructure-Documentation.md)** - Deployment and operational practices
- **[Environments & Deployment](./System-Documentation/4.-Infrastructure-Documentation/4.4-Environments-&-Deployment-Process.md)** - Two environments (Local, Production)

---

## Quick Start

1. Read the [System Documentation Overview](./System-Documentation.md) for architecture
2. Follow [Front-End Local Development Setup](./System-Documentation/1.-Front%2DEnd-Documentation/1.0-local_development_setup.md)
3. Follow [API Local Development Setup](./System-Documentation/2.-API-Documentation/2.0-local_development_setup.md)
4. Review [Business Processes](./Course-Enrollment-System/2.-Business-Processes.md) for requirements

---

## Architecture Highlights

- **Frontend:** Blazor WebAssembly (WASM)
- **Backend:** ASP.NET Core Minimal API
- **Database:** In-memory (EF Core)
- **Architecture:** Feature-based organization
- **Pattern:** Following Lesson 10.02 coding style

---

## Environments

The system supports two environments:

| Environment | Authentication | Database | Purpose |
|-------------|----------------|----------|---------|
| **Local** | JWT Bearer (Symmetric Key) | In-memory | Local development |
| **Production** | Entra ID (Azure AD) | In-memory | Live environment |

---

## Key Features

- Student Registration & Login
- Course Browsing
- Course Enrollment
- Enrollment Management (View & Deregister)
- CRUD Operations for Courses

---