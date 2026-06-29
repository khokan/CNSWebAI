# 📮 Postman Quick Start Guide

## 5-Minute Setup

### 1️⃣ Environment Setup (1 min)

```
Environments → + Create
├─ Name: CNSWebAI_Dev
├─ base_url = https://localhost:5001
└─ token = (empty)
```

### 2️⃣ Login Request (1 min)

```
POST {{base_url}}/api/auth/login

Headers:
  Content-Type: application/json

Body:
{
  "username": "admin",
  "password": "Admin@123"
}

Tests (auto-capture token):
pm.environment.set("token", pm.response.json().token);
```

**Click Send** → Token auto-saved ✓

---

### 3️⃣ Companies Request (1 min)

```
GET {{base_url}}/api/chatbot/companies

Authorization:
  Type: Bearer Token
  Token: {{token}}
```

**Click Send** → See 3 companies ✓

---

### 4️⃣ Query Request (1 min)

```
POST {{base_url}}/api/chatbot/query

Authorization:
  Type: Bearer Token
  Token: {{token}}

Body:
{
  "query": "What is the total turnover yearwise?",
  "companyId": null
}
```

**Click Send** → See turnover data ✓

---

### 5️⃣ History Request (1 min)

```
GET {{base_url}}/api/chatbot/history?pageNumber=1&pageSize=10

Authorization:
  Type: Bearer Token
  Token: {{token}}
```

**Click Send** → See your queries ✓

---

## Request Templates

### 📋 All Query Examples

**Query 1: Yearly Turnover**
```json
{
  "query": "What is the total turnover yearwise?",
  "companyId": null
}
```

**Query 2: TechCorp Turnover**
```json
{
  "query": "Show me TechCorp turnover by year",
  "companyId": 1
}
```

**Query 3: Quarterly Data**
```json
{
  "query": "What are the quarterly results?",
  "companyId": 1
}
```

**Query 4: Company Comparison**
```json
{
  "query": "Which company has the highest total turnover?",
  "companyId": null
}
```

**Query 5: Recent Data**
```json
{
  "query": "Show me recent financial data",
  "companyId": null
}
```

---

## Expected Responses

### ✅ Login Response
```json
{
  "success": true,
  "message": "Login successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "username": "admin",
    "email": "admin@cnsweb.com"
  }
}
```

### ✅ Companies Response
```json
{
  "success": true,
  "message": "Companies retrieved successfully",
  "data": [
    {
      "id": 1,
      "name": "TechCorp",
      "code": "TECH01"
    },
    {
      "id": 2,
      "name": "FinanceHub",
      "code": "FIN01"
    },
    {
      "id": 3,
      "name": "RetailPlus",
      "code": "RET01"
    }
  ]
}
```

### ✅ Query Response
```json
{
  "success": true,
  "message": "Query processed successfully",
  "data": {
    "query": "What is the total turnover yearwise?",
    "generatedSql": "SELECT YEAR(RecordDate) as Year...",
    "response": "Query executed successfully...",
    "isSuccessful": true,
    "data": [
      {
        "Year": 2024,
        "TotalTurnover": 8300000
      }
    ]
  }
}
```

### ✅ History Response
```json
{
  "success": true,
  "message": "Chat history retrieved successfully",
  "data": [
    {
      "id": 1,
      "userId": 1,
      "userQuery": "What is the total turnover yearwise?",
      "generatedSql": "SELECT YEAR(RecordDate)...",
      "isSuccessful": true,
      "createdAt": "2024-06-29T15:30:45"
    }
  ]
}
```

---

## ❌ Common Errors & Fixes

| Error | Fix |
|-------|-----|
| 401 Unauthorized | Login first, verify `{{token}}` in Authorization |
| Invalid username/password | Username: `admin`, Password: `Admin@123` |
| Connection refused | Start API: `dotnet run` in CNSWebAI.API |
| SSL certificate error | Postman Settings → SSL verification OFF |
| 400 Bad Request | Check JSON format in Body tab |
| Cannot POST /api/chatbot/query | Check URL spelling and {{base_url}} value |

---

## Request Checklist

Use this to verify all endpoints work:

### Authentication
- [ ] POST `/api/auth/login` - Get token
- [ ] POST `/api/auth/register` - Register new user
- [ ] GET `/api/auth/verify` - Verify token

### Chatbot
- [ ] GET `/api/chatbot/companies` - List companies
- [ ] POST `/api/chatbot/query` - Send query
- [ ] GET `/api/chatbot/history` - Get history

---

## Collection JSON (Import into Postman)

Save this as `CNSWebAI.postman_collection.json`:

```json
{
  "info": {
    "name": "CNSWebAI API",
    "description": "Complete Chatbot API Testing",
    "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "item": [
    {
      "name": "Auth - Login",
      "request": {
        "method": "POST",
        "header": [
          {"key": "Content-Type", "value": "application/json"}
        ],
        "body": {
          "mode": "raw",
          "raw": "{\"username\": \"admin\", \"password\": \"Admin@123\"}"
        },
        "url": {"raw": "{{base_url}}/api/auth/login", "protocol": "https"}
      }
    }
  ]
}
```

---

## Postman Pro Tips

### 💡 Tip 1: Save as Collection
Right-click requests → Add to Collection → Create "CNSWebAI"

### 💡 Tip 2: Use Runner
Click Runner → Select Collection → Run All

### 💡 Tip 3: Add Tests
Use Tests tab to validate responses automatically

### 💡 Tip 4: Pre-request Script
Set up data before sending requests

### 💡 Tip 5: Share Collection
Export → Send to team → Import in their Postman

---

## Testing Flow

```
1. Login (POST /auth/login)
    ↓
2. Companies (GET /chatbot/companies)
    ↓
3. Query 1 (POST /chatbot/query)
    ↓
4. Query 2 (POST /chatbot/query)
    ↓
5. History (GET /chatbot/history)
    ↓
✅ All tests pass!
```

---

## Company IDs for Queries

Use these in `companyId` field:

| Company | ID |
|---------|-----|
| TechCorp | 1 |
| FinanceHub | 2 |
| RetailPlus | 3 |
| All Companies | null |

---

## Sample Data Available

### TechCorp (ID: 1)
- 2024: 6.75M USD (Q1-Q4)
- 2023: 5.35M USD (Q1-Q4)

### FinanceHub (ID: 2)
- 2024: 8.75M USD (Q1-Q4)

### RetailPlus (ID: 3)
- 2024: 13.5M USD (Q1-Q4)

---

## Next Steps

1. ✅ Create Environment with variables
2. ✅ Create & test Login request
3. ✅ Create & test Companies request
4. ✅ Create & test Query requests (try all 5)
5. ✅ Create & test History request
6. ✅ Create Collection and save
7. ✅ Run all requests using Runner

---

**Start with Login request** → Copy token → Use in other requests → Done! 🎉
