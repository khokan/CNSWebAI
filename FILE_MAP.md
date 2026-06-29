# CNSWebAI - Project File Map & Structure

## Complete File Structure

```
I:\CSE\Projects\Current\WEB\ai\CNSWebAI\
│
├── 📄 CNSWebAI.sln                          [Solution file]
│
├── 📂 CNSWebAI.API\                         [Main Web API Project]
│   ├── 📂 Controllers\
│   │   ├── AuthController.cs                [Login, Register, Verify]
│   │   └── ChatbotController.cs             [Query, History, Companies]
│   │
│   ├── 📂 Middleware\
│   │   └── GlobalExceptionMiddleware.cs     [Error handling]
│   │
│   ├── 📂 Properties\
│   │   └── launchSettings.json              [Port & environment config]
│   │
│   ├── 📂 bin\                              [Compiled files - ignore]
│   ├── 📂 obj\                              [Temporary files - ignore]
│   │
│   ├── 📄 Program.cs                        [Startup & DI setup]
│   ├── 📄 appsettings.json                  [Configuration]
│   ├── 📄 appsettings.Development.json      [Development config]
│   └── 📄 CNSWebAI.API.csproj              [Project file]
│
├── 📂 CNSWebAI.Core\                        [Models & DTOs]
│   ├── 📂 Models\
│   │   ├── User.cs                          [User entity]
│   │   ├── Company.cs                       [Company entity]
│   │   ├── Turnover.cs                      [Financial data]
│   │   └── ChatHistory.cs                   [Query log]
│   │
│   ├── 📂 DTOs\
│   │   ├── AuthDtos.cs                      [Login/Register DTOs]
│   │   └── ChatDtos.cs                      [Chat request/response DTOs]
│   │
│   ├── 📂 bin\
│   ├── 📂 obj\
│   └── 📄 CNSWebAI.Core.csproj
│
├── 📂 CNSWebAI.Infrastructure\              [Data Access Layer]
│   ├── 📂 Data\
│   │   ├── ApplicationDbContext.cs          [EF Core DbContext]
│   │   │   └── ├── Entities configuration
│   │   │       ├── Relationships
│   │   │       └── Seed data
│   │   │
│   │   └── 📂 Repositories\
│   │       ├── Repository.cs                [Generic repository]
│   │       ├── UserRepository.cs            [User-specific queries]
│   │       └── TurnoverRepository.cs        [Turnover-specific queries]
│   │
│   ├── 📂 bin\
│   ├── 📂 obj\
│   ├── 📂 Migrations\                       [Database migrations]
│   │   └── [Auto-generated EF migration files]
│   │
│   └── 📄 CNSWebAI.Infrastructure.csproj
│
├── 📂 CNSWebAI.Services\                    [Business Logic]
│   ├── 📂 Authentication\
│   │   ├── JwtTokenService.cs               [Token generation]
│   │   └── AuthenticationService.cs         [Login/Register logic]
│   │
│   ├── 📂 LLM\
│   │   └── LocalLLMChatbotService.cs        [Query generation & processing]
│   │
│   ├── 📂 bin\
│   ├── 📂 obj\
│   └── 📄 CNSWebAI.Services.csproj
│
├── 📂 Frontend\                             [React Examples]
│   └── ChatbotComponent.jsx                 [React chatbot component]
│
├── 📂 logs\                                 [Application logs]
│   └── app-YYYY-MM-DD.txt                   [Daily log files]
│
├── 📚 Documentation Files
│   ├── README.md                            [Main documentation]
│   ├── SETUP.md                             [Installation & setup]
│   ├── API_TESTING.md                       [API testing examples]
│   ├── PROJECT_SUMMARY.md                   [Architecture overview]
│   ├── DEVELOPERS_GUIDE.md                  [Development guide]
│   ├── QUICK_REFERENCE.md                   [Quick reference card]
│   ├── CHANGELOG.md                         [Version history]
│   ├── FILE_MAP.md                          [This file]
│   └── PROJECT_FILE_MAP.md                  [Complete structure]
│
├── 📄 .gitignore                            [Git exclusions]
└── 📄 [Other configuration files]

```

## File Dependencies

```
Program.cs (Startup)
├── appsettings.json
├── AuthController.cs
│   └── AuthenticationService.cs
│       ├── JwtTokenService.cs
│       ├── IUserRepository
│       └── ApplicationDbContext
│
├── ChatbotController.cs
│   ├── IChatbotService
│   ├── IRepository<ChatHistory>
│   ├── ITurnoverRepository
│   └── IRepository<Company>
│
└── GlobalExceptionMiddleware.cs
```

## Project Dependencies

```
CNSWebAI.API
├── Depends on: CNSWebAI.Core
├── Depends on: CNSWebAI.Infrastructure
└── Depends on: CNSWebAI.Services

CNSWebAI.Services
├── Depends on: CNSWebAI.Core
└── References: System.IdentityModel.Tokens.Jwt

CNSWebAI.Infrastructure
├── Depends on: CNSWebAI.Core
└── References: Microsoft.EntityFrameworkCore.SqlServer

CNSWebAI.Core
└── No dependencies (lowest layer)

Frontend
└── No dependencies (standalone React)
```

## Database Schema Diagram

```
┌─────────────────────────┐
│        Users            │
├─────────────────────────┤
│ PK: Id                  │
│ FE: Username (Unique)   │
│ FE: Email (Unique)      │
│ .... PasswordHash       │
│ .... FullName           │
│ .... IsActive           │
└─────────────────────────┘
        │
        │ 1:N
        │
        ▼
┌─────────────────────────┐
│   ChatHistories         │
├─────────────────────────┤
│ PK: Id                  │
│ FK: UserId              │
│ .... UserQuery          │
│ .... GeneratedSql       │
│ .... Response           │
│ .... IsSuccessful       │
└─────────────────────────┘


┌─────────────────────────┐
│      Companies          │
├─────────────────────────┤
│ PK: Id                  │
│ FE: Code (Unique)       │
│ .... Name               │
│ .... Description        │
│ .... IsActive           │
└─────────────────────────┘
        │
        │ 1:N
        │
        ▼
┌─────────────────────────┐
│    Turnovers            │
├─────────────────────────┤
│ PK: Id                  │
│ FK: CompanyId           │
│ .... Amount (Decimal)   │
│ .... Year               │
│ .... Quarter            │
│ .... Currency           │
└─────────────────────────┘
```

## Code File Organization by Responsibility

### Authentication Files
- `AuthController.cs` → API endpoint
- `AuthenticationService.cs` → Business logic
- `JwtTokenService.cs` → Token handling
- `UserRepository.cs` → Data access
- `User.cs` → Model

### Chatbot Files
- `ChatbotController.cs` → API endpoint
- `LocalLLMChatbotService.cs` → Query processing
- `TurnoverRepository.cs` → Data access
- `Turnover.cs` → Model
- `ChatDtos.cs` → Data transfer objects

### Infrastructure Files
- `ApplicationDbContext.cs` → ORM mapping
- `Repository.cs` → Generic data access
- `UserRepository.cs` → User queries
- `TurnoverRepository.cs` → Turnover queries

## Configuration File Hierarchy

```
appsettings.json (Default)
    ├── Contains all default settings
    ├── Connection string
    ├── JWT settings
    └── Logging configuration
        │
        ▼
    appsettings.Development.json (Development Override)
        ├── Debug logging
        ├── Detailed errors
        └── Local settings
```

## API Request Flow (Mapped to Files)

```
HTTP Request
    │
    ▼
Program.cs (Routing)
    │
    ▼
AuthController.cs  OR  ChatbotController.cs
    │                         │
    ▼                         ▼
AuthenticationService      ChatbotService
    │                         │
    ├── JwtTokenService       ├── Query Generation
    ├── UserRepository        ├── TurnoverRepository
    │   │                     └── CompanyRepository
    │   ▼
    │ UserRepository
    │   │
    │   ▼
    │ ApplicationDbContext
    │   │
    │   ▼
    │ SQL Server
    │
    ▼
Response Object
    │
    ▼
GlobalExceptionMiddleware (Error handling)
    │
    ▼
HTTP Response
    │
    ▼
Client/Browser
```

## Entity Relationship Mapping

```
User (1) ──────→ (N) ChatHistory
          Owns  

Company (1) ──────→ (N) Turnover
            Owns
```

## Folder Purpose Summary

| Folder | Purpose | Files |
|--------|---------|-------|
| Controllers | API endpoints | 2 |
| Models | Domain entities | 4 |
| DTOs | Data contracts | 2 |
| Repositories | Data access | 3 |
| Services | Business logic | 2 |
| Middleware | Request processing | 1 |
| Properties | Build settings | 1 |
| Migrations | Database versioning | Auto-generated |
| logs | Application logs | Daily rolling |

## File Size Reference

```
Small Files (< 5 KB)
├── Models/* (User.cs, Company.cs, etc.)
└── DTOs/* (AuthDtos.cs, ChatDtos.cs)

Medium Files (5-15 KB)
├── Repositories/* (Repository.cs, etc.)
├── Services/* (JwtTokenService.cs, etc.)
└── Controllers/* (AuthController.cs, etc.)

Large Files (> 15 KB)
├── ApplicationDbContext.cs (Migrations config)
├── Program.cs (DI setup)
└── LocalLLMChatbotService.cs (Query logic)
```

## Critical Files (Must Not Delete)

```
🔴 Never delete:
├── CNSWebAI.sln                    (Solution definition)
├── Program.cs                      (Application startup)
├── ApplicationDbContext.cs         (Database mapping)
├── appsettings.json                (Configuration)
└── All model files                 (Data structure)

🟡 Be careful with:
├── Migrations/ folder              (Database history)
├── logs/ folder                    (Log files)
└── Database files                  (Data storage)
```

## Version Control Recommendations

### Commit These
```
✅ Source code (.cs files)
✅ Documentation (.md files)
✅ Configuration (appsettings.json)
✅ Project files (.csproj, .sln)
```

### Don't Commit
```
❌ bin/ and obj/ folders
❌ logs/ folder
❌ Database files
❌ Local settings
```

## Development Workflow Paths

### New Feature Development
```
1. Edit Domain Model (Models/*.cs)
   ↓
2. Create DTO (DTOs/*.cs)
   ↓
3. Update DbContext (ApplicationDbContext.cs)
   ↓
4. Create/Update Repository (Repositories/*.cs)
   ↓
5. Create Service (Services/*.cs)
   ↓
6. Add Controller Endpoint (Controllers/*.cs)
   ↓
7. Update Tests
   ↓
8. Run Migrations
```

### Bug Fix Workflow
```
1. Identify issue location
   ↓
2. Check logs (logs/*.txt)
   ↓
3. Review relevant code file
   ↓
4. Fix issue
   ↓
5. Test locally
   ↓
6. Commit with detailed message
```

## Quick File Lookup

**Need to...** → **Look in file...**

| Need to | File |
|---------|------|
| Add new entity | Models/*.cs |
| Add API endpoint | Controllers/*.cs |
| Change business logic | Services/*.cs |
| Access database | Repositories/*.cs |
| Configure app | appsettings.json |
| Handle errors | Middleware/*.cs |
| Change startup | Program.cs |
| Map entities | ApplicationDbContext.cs |

---

**Last Updated:** June 29, 2024
**Total Files:** 28
**Total Code Files:** 20
**Total Documentation:** 8
