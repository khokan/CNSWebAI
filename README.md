# CNSWebAI - AI Chatbot for Financial Data

A modern .NET 8 REST API with React frontend that provides natural language interface to query SQL Server financial data through an AI chatbot.

## Features

✅ **JWT Authentication** - Secure login and registration
✅ **Entity Framework Core** - ORM for SQL Server
✅ **Serilog Logging** - Comprehensive logging to file and console
✅ **Global Exception Handling** - Centralized error management
✅ **Local LLM Integration** - Query SQL generation from natural language
✅ **Chat History** - Track all user queries
✅ **Repository Pattern** - Clean data access layer
✅ **Dependency Injection** - Loosely coupled architecture
✅ **Swagger/OpenAPI** - Interactive API documentation

## Project Structure

```
CNSWebAI/
├── CNSWebAI.API/              # Web API (Entry point)
│   ├── Controllers/           # API endpoints
│   ├── Middleware/            # Exception handling, logging
│   ├── Program.cs            # Startup configuration
│   └── appsettings.json      # Configuration
│
├── CNSWebAI.Core/            # Business models & DTOs
│   ├── Models/               # Domain entities
│   └── DTOs/                 # Data transfer objects
│
├── CNSWebAI.Infrastructure/  # Data access layer
│   ├── Data/                 # DbContext & Repositories
│   └── Repositories/         # Repository implementations
│
├── CNSWebAI.Services/        # Business logic services
│   ├── Authentication/       # Auth & JWT
│   └── LLM/                  # Chatbot/Query generation
│
└── Frontend/                 # React components
    └── ChatbotComponent.jsx
```

## Tech Stack

- **.NET 8** - Latest .NET framework
- **Entity Framework Core 8** - ORM
- **SQL Server** - Database
- **JWT** - Authentication
- **Serilog** - Logging
- **Swagger** - API Documentation
- **React** - Frontend (example)
- **BCrypt** - Password hashing

## Getting Started

### Prerequisites

- .NET 8 SDK or later
- SQL Server 2019 or later
- Visual Studio 2022 or VS Code

### Installation

1. **Clone or extract the project**
```bash
cd CNSWebAI
```

2. **Restore NuGet packages**
```bash
cd CNSWebAI.API
dotnet restore
```

3. **Update Database Connection**
Edit `appsettings.json`:
```json
"DefaultConnection": "server=YOUR_SERVER;database=YOUR_DB;uid=YOUR_USER;password=YOUR_PASSWORD;..."
```

4. **Run Migrations**
```bash
dotnet ef database update -p ../CNSWebAI.Infrastructure
```

5. **Start the API**
```bash
dotnet run
```

The API will be available at: `https://localhost:5001`
Swagger UI: `https://localhost:5001/swagger`

## API Endpoints

### Authentication

#### Login
```
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin@123"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Login successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "username": "admin",
    "email": "admin@cnsweb.com",
    "fullName": "Administrator",
    "isActive": true
  }
}
```

#### Register
```
POST /api/auth/register
Content-Type: application/json

{
  "username": "newuser",
  "email": "user@example.com",
  "password": "Password@123",
  "fullName": "John Doe"
}
```

### Chatbot

#### Send Query
```
POST /api/chatbot/query
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

{
  "query": "What is the total turnover yearwise?",
  "companyId": null
}
```

**Response:**
```json
{
  "success": true,
  "message": "Query processed successfully",
  "data": {
    "chatHistoryId": 1,
    "query": "What is the total turnover yearwise?",
    "response": "Query executed successfully...",
    "generatedSql": "SELECT YEAR(RecordDate)...",
    "isSuccessful": true,
    "data": {...}
  }
}
```

#### Get Chat History
```
GET /api/chatbot/history?pageNumber=1&pageSize=10
Authorization: Bearer YOUR_JWT_TOKEN
```

#### Get Companies
```
GET /api/chatbot/companies
Authorization: Bearer YOUR_JWT_TOKEN
```

## Sample Data

The database is seeded with:

- **3 Companies**: TechCorp, FinanceHub, RetailPlus
- **16 Turnover Records**: Quarterly data for 2023-2024
- **1 Test User**: 
  - Username: `admin`
  - Password: `Admin@123`
  - Email: `admin@cnsweb.com`

## Database Schema

### Users
```sql
CREATE TABLE Users (
  Id INT PRIMARY KEY IDENTITY,
  Username NVARCHAR(100) UNIQUE,
  Email NVARCHAR(100) UNIQUE,
  FullName NVARCHAR(255),
  PasswordHash NVARCHAR(MAX),
  IsActive BIT,
  CreatedAt DATETIME,
  UpdatedAt DATETIME
)
```

### Companies
```sql
CREATE TABLE Companies (
  Id INT PRIMARY KEY IDENTITY,
  Name NVARCHAR(255),
  Code NVARCHAR(50) UNIQUE,
  Description NVARCHAR(500),
  IsActive BIT,
  CreatedAt DATETIME
)
```

### Turnovers
```sql
CREATE TABLE Turnovers (
  Id INT PRIMARY KEY IDENTITY,
  CompanyId INT FOREIGN KEY,
  Amount DECIMAL(18,2),
  Year INT,
  Quarter INT,
  Currency NVARCHAR(10),
  Description NVARCHAR(500),
  RecordDate DATETIME
)
```

### ChatHistories
```sql
CREATE TABLE ChatHistories (
  Id INT PRIMARY KEY IDENTITY,
  UserId INT FOREIGN KEY,
  UserQuery NVARCHAR(MAX),
  GeneratedSql NVARCHAR(MAX),
  Response NVARCHAR(MAX),
  RawQueryResult NVARCHAR(MAX),
  IsSuccessful BIT,
  ErrorMessage NVARCHAR(MAX),
  CreatedAt DATETIME
)
```

## Logging

Logs are written to:
- **Console** - Real-time output
- **File** - `logs/app-{date}.txt` (daily rolling)

## Configuration

### JWT Settings
Edit `appsettings.json`:
```json
"JwtSettings": {
  "Secret": "your-secret-key-at-least-32-characters",
  "Issuer": "CNSWebAI",
  "Audience": "CNSWebAIUsers",
  "ExpiryHours": 24
}
```

## Query Examples for Chatbot

The chatbot recognizes these natural language patterns:

1. **Yearly Turnover**
   - "What is the total turnover yearwise?"
   - "Show me turnover by year"

2. **Quarterly Data**
   - "Show quarterly turnover"
   - "What are the quarterly results?"

3. **Company Totals**
   - "Which company has highest turnover?"
   - "Total turnover by company"

4. **Latest Data**
   - "Show recent turnover"
   - "Latest financial data"

## React Frontend Integration

Install dependencies:
```bash
npm install axios react-router-dom
```

Usage example:
```jsx
import ChatbotComponent from './ChatbotComponent';

function App() {
  return <ChatbotComponent />;
}
```

## Development

### Building
```bash
dotnet build
```

### Running Tests
```bash
dotnet test
```

### Database Migrations
```bash
# Add migration
dotnet ef migrations add MigrationName -p ../CNSWebAI.Infrastructure

# Update database
dotnet ef database update -p ../CNSWebAI.Infrastructure

# Revert migration
dotnet ef database update PreviousMigration -p ../CNSWebAI.Infrastructure
```

## Security

- Passwords hashed with BCrypt
- JWT token-based authentication
- Connection string encrypted
- Global exception handling (no sensitive info leaked)
- Input validation on all endpoints

## Performance Optimization

- Entity Framework query optimization
- Pagination support
- Indexed queries
- Async/await throughout
- Connection pooling

## Error Handling

Global middleware handles:
- 401: Unauthorized
- 400: Bad Request
- 500: Internal Server Error
- Custom exceptions

## Future Enhancements

- [ ] Integration with Ollama/LM Studio for local LLMs
- [ ] Advanced query caching
- [ ] Real-time WebSocket support
- [ ] Advanced analytics dashboard
- [ ] Export to CSV/Excel
- [ ] Scheduled reports
- [ ] Multi-language support
- [ ] Rate limiting

## License

Proprietary - CNS Technologies

## Support

For issues and questions, contact your administrator.
