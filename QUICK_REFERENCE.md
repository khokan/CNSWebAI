# CNSWebAI - Quick Reference Card

## 🚀 Quick Start (5 Minutes)

```bash
# 1. Open command prompt at solution folder
cd I:\CSE\Projects\Current\WEB\ai\CNSWebAI

# 2. Restore packages
dotnet restore

# 3. Update database
cd CNSWebAI.API
dotnet ef database update --project ../CNSWebAI.Infrastructure

# 4. Run API
dotnet run

# 5. Access Swagger
# Open: https://localhost:5001/swagger
```

## 🔐 Default Login
```
Username: admin
Password: Admin@123
```

## 📝 Essential Endpoints

| Method | Endpoint | Auth | Purpose |
|--------|----------|------|---------|
| POST | /api/auth/login | ❌ | Login & get token |
| POST | /api/auth/register | ❌ | Register new user |
| POST | /api/chatbot/query | ✅ | Send query to chatbot |
| GET | /api/chatbot/history | ✅ | Get chat history |
| GET | /api/chatbot/companies | ✅ | List companies |

## 🔑 Configuration

**File:** `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=YOUR_SERVER;database=CNSWeb;uid=YOUR_USER;password=YOUR_PASSWORD;"
  },
  "JwtSettings": {
    "Secret": "your-32-character-secret-key",
    "ExpiryHours": 24
  }
}
```

## 📊 Database

**Connection String Format:**
```
server=192.168.102.21;database=CNSWeb;uid=cseweb;password=password;Encrypt=true;TrustServerCertificate=true;
```

**Check Data:**
```sql
SELECT * FROM Companies        -- 3 sample companies
SELECT * FROM Turnovers       -- 16 sample records
SELECT * FROM Users           -- 1 test user
SELECT * FROM ChatHistories   -- User queries log
```

## 🛠️ Common Commands

```bash
# Build
dotnet build

# Run with watch mode
dotnet watch run

# Create migration
dotnet ef migrations add MigrationName --project ../CNSWebAI.Infrastructure

# Update database
dotnet ef database update --project ../CNSWebAI.Infrastructure

# View logs
Get-Content logs\app-*.txt -Tail 50

# Reset database (WARNING: Data loss)
dotnet ef database drop -f
dotnet ef database update --project ../CNSWebAI.Infrastructure
```

## 🧪 Test API with cURL

### Login
```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin@123"}'
```

### Send Query (replace TOKEN)
```bash
curl -X POST https://localhost:5001/api/chatbot/query \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TOKEN_HERE" \
  -d '{"query":"What is the total turnover yearwise?","companyId":null}'
```

## 📁 Project Structure

```
CNSWebAI/
├── CNSWebAI.API/           → Main API project
├── CNSWebAI.Core/          → Models & DTOs
├── CNSWebAI.Infrastructure/→ Database & Repositories
├── CNSWebAI.Services/      → Business logic
├── Frontend/               → React examples
└── Documentation files
```

## 🔍 Finding Things

| Need to find... | Location |
|-----------------|----------|
| Models | `CNSWebAI.Core/Models/` |
| Controllers | `CNSWebAI.API/Controllers/` |
| Database logic | `CNSWebAI.Infrastructure/Data/` |
| Services | `CNSWebAI.Services/` |
| Configuration | `CNSWebAI.API/appsettings.json` |

## ⚠️ Common Issues

| Issue | Solution |
|-------|----------|
| Connection failed | Check connection string, verify SQL Server running |
| JWT token invalid | Token expired or secret key mismatch |
| Database not found | Run migrations: `dotnet ef database update` |
| Port already in use | Change port in `launchSettings.json` |
| CORS error | Check frontend URL in CORS policy |

## 📚 Documentation Files

- **README.md** - Full documentation
- **SETUP.md** - Detailed setup guide
- **API_TESTING.md** - API examples & testing
- **PROJECT_SUMMARY.md** - Architecture overview
- **DEVELOPERS_GUIDE.md** - Development guide
- **QUICK_REFERENCE.md** - This file

## 🔐 JWT Token Usage

After login, you get a token. Use it in subsequent requests:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

Token expires in 24 hours (configurable).

## 🚨 Error Codes

| Code | Meaning |
|------|---------|
| 200 | ✅ Success |
| 400 | ❌ Bad request (invalid data) |
| 401 | ❌ Unauthorized (missing/invalid token) |
| 404 | ❌ Not found |
| 500 | ❌ Server error |

## 💾 Backup & Restore

**Backup Database:**
```bash
sqlcmd -S YOUR_SERVER -U YOUR_USER -P YOUR_PASSWORD ^
       -Q "BACKUP DATABASE CNSWeb TO DISK = 'C:\Backup\CNSWeb.bak'"
```

**Restore Database:**
```bash
sqlcmd -S YOUR_SERVER -U YOUR_USER -P YOUR_PASSWORD ^
       -Q "RESTORE DATABASE CNSWeb FROM DISK = 'C:\Backup\CNSWeb.bak'"
```

## 🔗 Useful Links

- Swagger UI: `https://localhost:5001/swagger`
- API Health Check: `https://localhost:5001/api/auth/verify` (requires token)
- Logs: `./logs/app-{date}.txt`

## 📞 Support

Check files in order:
1. SETUP.md - Installation issues
2. API_TESTING.md - API issues
3. DEVELOPERS_GUIDE.md - Development issues
4. logs/app-*.txt - Error details

## ⏱️ Expected Response Times

| Operation | Time |
|-----------|------|
| Login | 100-150ms |
| Query | 200-500ms |
| Database read | 50-100ms |
| Token validation | <10ms |

## 🎯 Next Steps

1. ✅ Run the API
2. ✅ Test login in Swagger
3. ✅ Send a chatbot query
4. ✅ Check the logs
5. ✅ View database changes
6. ✅ Build React frontend
7. ✅ Deploy to production

## 🔐 Security Reminders

- ✅ Never commit passwords
- ✅ Use strong JWT secret (32+ chars)
- ✅ Change default admin password
- ✅ Enable HTTPS in production
- ✅ Use parameterized queries
- ✅ Validate all inputs
- ✅ Keep logs secure
- ✅ Update dependencies regularly

## Version Info

- **.NET:** 8.0
- **SQL Server:** 2019+
- **Created:** June 29, 2024
- **Status:** ✅ Production Ready

---

**Need help?** Check the documentation files or review the code comments!

**Questions about Claude/AI?** Check the LLM Service implementation in `LocalLLMChatbotService.cs`
