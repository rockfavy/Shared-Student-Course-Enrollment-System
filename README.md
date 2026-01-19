# Student Course Enrollment System

A Blazor WebAssembly application with a Minimal API backend for managing student course enrollments.

## Quick Setup

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- A code editor (Visual Studio, Visual Studio Code, or Rider)

### Installation Steps

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd "Student Course Enrollment System"
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the application**
   
   **Start the API:**
   ```bash
   cd src/StudentCourseEnrollment.Api
   dotnet run
   ```
   The API will be available at:
   - HTTPS: `https://localhost:49541`
   - HTTP: `http://localhost:49542`
   - Swagger UI: `https://localhost:49541/swagger`

   **Start the Frontend** (in a new terminal):
   ```bash
   cd src/StudentCourseEnrollment.Frontend
   dotnet run
   ```
   The frontend will be available at:
   - HTTPS: `https://localhost:5001`
   - HTTP: `http://localhost:5000`

4. **Access the application**
   - Open your browser and navigate to `https://localhost:5001`
   - The application will automatically seed sample data including an admin account

### Default Admin Credentials

- **Email:** `admin@example.com`
- **Password:** `Admin123!`

> **Note:** The admin account is automatically created when the application starts. Use these credentials to log in and access administrative features.

### Environment Configuration

The application supports different environments:

- **Development:** Uses custom JWT authentication (default)
- **Production:** Uses Entra ID (Azure AD) authentication

To run in Production mode, set the `ASPNETCORE_ENVIRONMENT` environment variable to `Production` before starting the application.

---

## Project Structure

This section provides an overview of the project directory structure for developers who need to navigate or extend the codebase.

### Root Directory

```
Student Course Enrollment System/
├── README.md                          # This file
├── StudentCourseEnrollment.sln        # Visual Studio solution file
├── .gitignore                         # Git ignore rules
├── devops/                            # CI/CD pipeline definitions
│   └── pipelines/
│       ├── ci.yml                     # Continuous Integration pipeline
│       └── prod-deploy.yml           # Production deployment pipeline
├── src/                               # Source code
│   ├── StudentCourseEnrollment.Api/   # Backend API (Minimal API)
│   ├── StudentCourseEnrollment.Frontend/  # Frontend (Blazor WebAssembly)
│   ├── StudentCourseEnrollment.Shared/   # Shared DTOs and models
│   └── tests/                         # Test projects
│       ├── StudentCourseEnrollment.Tests.Integration/
│       └── StudentCourseEnrollment.Tests.Unit/
└── Student-Course-Enrollment-System.wiki/  # Comprehensive documentation
```

### Source Code (`src/`)

#### `StudentCourseEnrollment.Api/`
The backend Minimal API project containing:
- **Features/**: Feature-based organization (Auth, Courses, Enrollments)
- **Models/**: Entity models (Student, Course, Enrollment)
- **Data/**: Database context and seeding logic
- **Program.cs**: Application entry point and configuration

#### `StudentCourseEnrollment.Frontend/`
The Blazor WebAssembly frontend containing:
- **Components/**: Razor components (Pages, Layout, Shared)
- **Clients/**: API client interfaces and implementations
- **Services/**: Authentication state provider
- **Helpers/**: Utility classes (JWT token parsing)
- **wwwroot/**: Static files (CSS, HTML)

#### `StudentCourseEnrollment.Shared/`
Shared project containing:
- **DTOs/**: Data Transfer Objects for API communication
- **Role.cs**: Role enumeration

#### `tests/`
Test projects:
- **StudentCourseEnrollment.Tests.Integration/**: Integration tests
- **StudentCourseEnrollment.Tests.Unit/**: Unit tests

### DevOps (`devops/`)

Contains Azure DevOps pipeline definitions:
- **ci.yml**: Continuous Integration pipeline (builds and tests on feature branch)
- **prod-deploy.yml**: Production deployment pipeline (builds, tests, and deploys to production)

### Documentation (`Student-Course-Enrollment-System.wiki/`)

Comprehensive system documentation including:
- Business overview and processes
- Frontend documentation
- API documentation
- Database schema
- Infrastructure and deployment guides

---

## Features

- **Student Registration & Login**: Students can create accounts and authenticate
- **Course Browsing**: View available courses with capacity information
- **Course Enrollment**: Students can enroll in courses (subject to capacity)
- **Enrollment Management**: Students can view and deregister from enrolled courses
- **Course Management**: Administrators can create, update, and delete courses
- **Role-Based Access Control**: Different permissions for Students and Administrators

---

## Testing

Run all tests:
```bash
dotnet test
```

Run specific test projects:
```bash
# Unit tests
dotnet test src/tests/StudentCourseEnrollment.Tests.Unit

# Integration tests
dotnet test src/tests/StudentCourseEnrollment.Tests.Integration
```

---

## Additional Resources

For detailed documentation, architecture information, and development guidelines, refer to the comprehensive wiki documentation in the `Student-Course-Enrollment-System.wiki/` directory.

---

## Hosted Version

To request access to the hosted production version of this application, please send an email to:

**rock.learn@favysoft.com**

Include your name, email address, and the reason for requesting access.

---




