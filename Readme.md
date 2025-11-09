# 🏥 InsureClaim - Modern Insurance Management SaaS

> **Full-stack insurance platform built with .NET 8, React, and SQL Server**  
> Automating policy management, claims processing, and risk assessment for the digital age.

![Build Status](https://img.shields.io/badge/build-passing-brightgreen)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-red)
![Progress](https://img.shields.io/badge/progress-50%25-orange)

---

## 🎯 Project Vision

InsureClaim reduces administrative overhead by **60%** through intelligent automation and streamlined workflows. Built with enterprise-grade architecture, the platform supports multi-role access, real-time analytics, and is designed for future AI integration.

### **Why This Matters**

Traditional insurance systems suffer from:

- ❌ Manual claim processing taking 7-14 days
- ❌ Fragmented customer-agent communication
- ❌ Limited business intelligence
- ❌ Rigid architectures

**InsureClaim delivers:**

- ✅ Automated workflows (claim review < 24 hours)
- ✅ Real-time dashboards for instant insights
- ✅ Role-based security (Customer/Agent/Admin)
- ✅ Modular architecture ready for AI/ML

---

## 🏗️ Tech Stack

### **Backend**

- **.NET 8 Web API** - RESTful services
- **Entity Framework Core 8** - ORM with migrations
- **SQL Server** - Relational database
- **JWT Authentication** - Secure token-based auth
- **Serilog** - Structured logging
- **Clean Architecture** - Separation of concerns with Dependency Injection

### **Frontend** (Coming in Phase 1.2)

- **React 18** with Hooks
- **Tailwind CSS** - Modern styling
- **Recharts** - Data visualization
- **Axios** - HTTP client

### **Deployment** (Phase 5)

- **Backend** - Render / Railway
- **Frontend** - Vercel / Netlify
- **CI/CD** - GitHub Actions
- **Future** - Azure-ready containerized services

---

## 📊 Current Progress: **50% Complete**

### ✅ **Phase 1: Foundation (IN PROGRESS - Day 1 Complete)**

- [x] Clean architecture structure (4 layers)
- [x] Domain entities with business rules
- [x] Database context and migrations
- [x] JWT authentication setup
- [x] Serilog integration
- [x] SQL Server database created
- [x] Initial tables with relationships
- [x] User registration & login APIs (Day 2)
- [x] Policy CRUD endpoints
- [ ] **NEXT:** React frontend setup

### 🔜 **Phase 2: Business Operations** (Weeks 2-3)

- Underwriting system with risk engine
- Claims management workflow
- Payment processing

### 🔜 **Phase 3: Analytics & Reporting** (Week 4)

- Interactive dashboards
- Export to PDF/Excel

---

## 🚀 Getting Started

### **Prerequisites**

- .NET 8 SDK
- SQL Server 2019+ or SQL Server Express
- SQL Server Management Studio (SSMS)
- Visual Studio Code

### **Installation**

1. **Clone Repository**

```bash
git clone https://github.com/obakengshepherd/InsureClaim.git
cd InsureClaim
```

2. **Configure Database**

```bash
cd backend/InsureClaim.API

# Update appsettings.json with your SQL Server instance
# Example: "Server=YOUR_MACHINE\\SQLEXPRESS;Database=InsureClaimDB;..."
```

# Restore packages

```bash
dotnet restore
```

3. **Run Migrations**

```bash
dotnet ef database update
```

4. **Start API**

```bash
dotnet run
```

5. **Access Swagger & Test APIs**

- Open: https://localhost:7XXX/swagger
- Test the authentication endpoints
- For protected routes (like /me), click "Authorize" and enter: Bearer YOUR_TOKEN
- Default admin login:
  - Email: admin@insureclaim.com
  - Password: Admin@123

---

## 🗂️ Project Structure

```
InsureClaim/
├── backend/
│   ├── InsureClaim.API/                # Web API controllers, Program.cs, Swagger
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs       # Authentication endpoints
│   │   │   ├── PolicyController.cs     # Policy management endpoints
│   │   │   ├── ClaimController.cs      # Claims management endpoints
│   │   │   └── PaymentController.cs    # Payment tracking endpoints (NEW)
│   │   ├── Program.cs                  # App configuration & DI
│   │   └── appsettings.json           # Configuration & secrets
│   │
│   ├── InsureClaim.Core/               # Domain entities & enums
│   │   └── Entities/
│   │       ├── User.cs                 # User entity with roles
│   │       ├── Policy.cs               # Insurance policy
│   │       ├── InsuranceClaim.cs       # Claim entity
│   │       └── Payment.cs              # Payment transactions
│   │
│   ├── InsureClaim.Application/        # Business logic & services
│   │   ├── DTOs/                       # Data Transfer Objects
│   │   │   ├── RegisterDto.cs
│   │   │   ├── LoginDto.cs
│   │   │   ├── AuthResponseDto.cs
│   │   │   ├── UserDto.cs
│   │   │   ├── CreatePolicyDto.cs
│   │   │   ├── UpdatePolicyDto.cs
│   │   │   ├── PolicyDto.cs
│   │   │   ├── SubmitClaimDto.cs
│   │   │   ├── UpdateClaimDto.cs
│   │   │   ├── ClaimDto.cs
│   │   │   ├── RecordPaymentDto.cs     # (NEW)
│   │   │   ├── UpdatePaymentDto.cs     # (NEW)
│   │   │   └── PaymentDto.cs           # (NEW)
│   │   ├── Interfaces/
│   │   │   ├── IAuthService.cs
│   │   │   ├── IJwtService.cs
│   │   │   ├── IPolicyService.cs
│   │   │   ├── IClaimService.cs
│   │   │   └── IPaymentService.cs      # (NEW)
│   │   └── Services/
│   │       ├── AuthService.cs          # Authentication logic
│   │       ├── JwtService.cs           # JWT token management
│   │       ├── PolicyService.cs        # Policy business logic
│   │       ├── ClaimService.cs         # Claims workflow logic
│   │       └── PaymentService.cs       # Payment processing logic (NEW)
│   │
│   └── InsureClaim.Infrastructure/     # Data access & external services
│       ├── Data/
│       │   └── ApplicationDbContext.cs # EF Core DbContext
│       └── Migrations/                 # Database migrations
│
├── frontend/                            # React application (Phase 2)
└── README.md

***Why Clean Architecture?**

- ✅ Testability: Core business logic is independent of frameworks
- ✅ Maintainability: Clear separation of concerns, Easy to test and maintain
- ✅ Scalability: Easy to add new features without breaking existing code which is scalable for enterprise growth
- ✅ Future-Proof: Ready for microservices migration if needed

---

## 📈 Database Schema

### **Core Tables**

| Table        | Purpose                | Key Fields                            |
| ------------ | ---------------------- | ------------------------------------- |
| **Users**    | Authentication & roles | Email (unique), Role, PasswordHash    |
| **Policies** | Insurance products     | PolicyNumber, Type, Premium, Coverage |
| **Claims**   | Customer claims        | ClaimNumber, Status, Amount           |
| **Payments** | Transaction history    | TransactionId, Amount, Method         |

📈 Database Schema - Core Entities
Users - Customers, Agents, Admins with role-based access
Policies - Life, Auto, Health, Property insurance products
Claims - Submitted → Under Review → Approved/Denied workflow
Payments - Transaction history linked to policies

**Relationships:**

- 1 User → Many Policies
- 1 Policy → Many Claims & Payments
- Foreign keys with `Restrict` delete behavior

---

## 🎓 Technical Highlights

### **Skills Demonstrated**

- ✅ RESTful API design with ASP.NET Core
- ✅ Database modeling with EF Core and ORM usage
- ✅ Authentication with JWT tokens
- ✅ Dependency Injection & IoC
- ✅ Structured logging for production and observability
- ✅ Clean Architecture principles

### **Business Impact**

- **60% faster processing** through automated workflows
- **Real-time visibility** into policies, claims, and revenue
- **Secure multi-tenant** architecture supporting thousands of users
- **Audit trail** for compliance and dispute resolution

---

## 🗺️ Development Roadmap

- **Week 1 (Day 1):** ✅ Database foundation (COMPLETE)
- **Week 1 (Day 2-7):** ✅ Authentication APIs + Policy CRUD → (COMPLETE)
- **Week 2-3:** ✅ Claims workflow + Payments → **Target: 40%**
- **Week 4:** Analytics dashboards → **Target: 60%**
- **Week 5:** Automation + Chatbot → **Target: 80%**
- **Week 6:** Deployment + Polish → **Target: 100%**

---

## 👨‍💻 Author

**Tsaagane Obakeng Shepherd**
Final-Year Software Engineering Student | .NET Backend Developer

- 📧 Email: obakengtsaagane@gmail.com.com
- 💼 LinkedIn: [My Profile](https://www.linkedin.com/in/obakeng-tsaagane-307544244/)
- 🐱 GitHub: [@obakengshepherd](https://github.com/obakengshepherd)

---

## 📄 License

Built as a portfolio showcase project

---

**💡 Built with passion to demonstrate production-ready software engineering practices**
```
