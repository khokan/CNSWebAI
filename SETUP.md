# CNSWebAI - Quick Setup Guide

## Step-by-Step Setup Instructions

### Step 1: Prerequisites Check
```bash
# Check .NET version (should be 8.0 or later)
dotnet --version

# Check SQL Server is running
# Verify connection string in appsettings.json
```

### Step 2: Update Configuration

Edit `CNSWebAI.API/appsettings.json` and update the connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "server=YOUR_SERVER;database=CNSWeb;uid=cseweb;password=YOUR_PASSWORD;Encrypt=true;TrustServerCertificate=true;Connection Timeout=30;"
}
```

Also update JWT Secret (minimum 32 characters):
```json
"JwtSettings": {
  "Secret": "your-super-secret-key-minimum-32-characters-long!"
}
```

### Step 3: Restore Packages

Open command prompt in the solution directory:
```bash
dotnet restore
```

### Step 4: Create Database and Run Migrations

```bash
cd CNSWebAI.API
dotnet ef database update --project ../CNSWebAI.Infrastructure --startup-project .
```

This will:
- Create the database if it doesn't exist
- Run all migrations
- Seed sample data automatically

### Step 5: Build the Solution

```bash
dotnet build
```

### Step 6: Run the API

```bash
cd CNSWebAI.API
dotnet run
```

Expected output:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to stop, restarting...
```

### Step 7: Test the API

Open browser and navigate to:
```
https://localhost:5001/swagger
```

You should see Swagger UI with all API endpoints.

### Step 8: Login with Test Account

In Swagger UI, find the `/api/auth/login` endpoint and execute:

```json
{
  "username": "admin",
  "password": "Admin@123"
}
```

Copy the returned token.

### Step 9: Authorize in Swagger

1. Click the "Authorize" button (top right)
2. Paste the token with `Bearer ` prefix:
   ```
   Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
   ```
3. Click Authorize

### Step 10: Test Chatbot Endpoint

1. Find `/api/chatbot/query` endpoint
2. Execute with sample query:
   ```json
   {
     "query": "What is the total turnover yearwise?",
     "companyId": null
   }
   ```

Expected response:
```json
{
  "success": true,
  "message": "Query processed successfully",
  "data": {
    "query": "What is the total turnover yearwise?",
    "response": "Query executed successfully...",
    "generatedSql": "SELECT YEAR(RecordDate)...",
    "data": {...}
  }
}
```

## Troubleshooting

### Issue: Database connection failed
**Solution:**
- Verify server name and credentials in `appsettings.json`
- Check SQL Server is running
- Test connection in SQL Server Management Studio

### Issue: EF migrations error
**Solution:**
```bash
# Clean and rebuild
dotnet clean
dotnet build

# Retry migrations
dotnet ef database drop -f
dotnet ef database update --project ../CNSWebAI.Infrastructure
```

### Issue: JWT token error
**Solution:**
- Ensure JWT Secret is at least 32 characters
- Check token hasn't expired (24 hours by default)
- Verify token format in Authorization header: `Bearer <token>`

### Issue: CORS error on frontend
**Solution:**
- Verify `AllowReact` CORS policy is configured in Program.cs
- Check frontend URL matches allowed origins
- Browser console should show exact error

### Issue: Port already in use
**Solution:**
```bash
# Change port in Properties/launchSettings.json
# Or kill process on port 5001:
netstat -ano | findstr :5001
taskkill /PID <PID> /F
```

## Database Troubleshooting

### Check if database exists:
```sql
SELECT name FROM sys.databases WHERE name = 'CNSWeb'
```

### Check tables:
```sql
USE CNSWeb
SELECT * FROM INFORMATION_SCHEMA.TABLES
```

### View sample data:
```sql
SELECT * FROM Companies
SELECT * FROM Turnovers
SELECT * FROM Users
```

### Reset database (CAUTION - Data Loss):
```bash
dotnet ef database drop -f
dotnet ef database update
```

## Useful Commands

### View logs in real-time:
```bash
Get-Content logs\app-*.txt -Wait -Tail 20
```

### Create a new migration:
```bash
dotnet ef migrations add MigrationName --project ../CNSWebAI.Infrastructure
```

### Update database with specific migration:
```bash
dotnet ef database update MigrationName --project ../CNSWebAI.Infrastructure
```

### Check current migrations:
```bash
dotnet ef migrations list --project ../CNSWebAI.Infrastructure
```

## Performance Testing

### Load test the API:
```bash
# Using Apache Bench
ab -n 1000 -c 10 https://localhost:5001/api/chatbot/companies

# Using curl
curl -X GET https://localhost:5001/api/auth/verify \
  -H "Authorization: Bearer YOUR_TOKEN"
```

## Next Steps

1. **Customize LLM queries** in `ChatbotService.cs`
2. **Add more entities** to the data model
3. **Integrate real LLM** (Ollama, LM Studio, etc.)
4. **Build React frontend** using the provided component
5. **Add authentication UI** for login/register
6. **Deploy to production** (Azure, AWS, etc.)

## Getting Help

Check logs for detailed error information:
```bash
Get-Content logs\app-*.txt
```

API Error responses include:
- `message`: Human-readable error message
- `errors`: Detailed field-level errors (if applicable)
- HTTP status code indicates error type

## Environment Variables (Optional)

Create `.env` file for sensitive data:
```
JWT_SECRET=your-secret-key
DB_CONNECTION_STRING=your-connection-string
```

Then in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "${DB_CONNECTION_STRING}"
}
```

Load from environment in Program.cs:
```csharp
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? 
                builder.Configuration["JwtSettings:Secret"];
```
