# CNSWebAI API Testing Guide

## Postman/cURL Examples

### 1. Authentication

#### Register New User
```bash
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "TestPassword@123",
    "fullName": "Test User"
  }'
```

#### Login
```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "Admin@123"
  }'
```

Response:
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

**Save the token for subsequent requests!**

### 2. Chatbot Queries

Set variable: `TOKEN=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`

#### Query 1: Yearly Turnover for All Companies
```bash
curl -X POST https://localhost:5001/api/chatbot/query \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "query": "What is the total turnover yearwise?",
    "companyId": null
  }'
```

#### Query 2: Specific Company Turnover
```bash
curl -X POST https://localhost:5001/api/chatbot/query \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "query": "Show me TechCorp turnover by year",
    "companyId": 1
  }'
```

#### Query 3: Quarterly Data
```bash
curl -X POST https://localhost:5001/api/chatbot/query \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "query": "What are the quarterly results?",
    "companyId": 1
  }'
```

#### Query 4: Company Comparison
```bash
curl -X POST https://localhost:5001/api/chatbot/query \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "query": "Which company has the highest total turnover?",
    "companyId": null
  }'
```

#### Query 5: Latest Data
```bash
curl -X POST https://localhost:5001/api/chatbot/query \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "query": "Show me recent financial data",
    "companyId": null
  }'
```

### 3. Get Companies
```bash
curl -X GET https://localhost:5001/api/chatbot/companies \
  -H "Authorization: Bearer $TOKEN"
```

Response:
```json
{
  "success": true,
  "message": "Companies retrieved successfully",
  "data": [
    {
      "id": 1,
      "name": "TechCorp",
      "code": "TECH01",
      "description": "Technology Solutions"
    },
    {
      "id": 2,
      "name": "FinanceHub",
      "code": "FIN01",
      "description": "Financial Services"
    },
    {
      "id": 3,
      "name": "RetailPlus",
      "code": "RET01",
      "description": "Retail Operations"
    }
  ]
}
```

### 4. Get Chat History
```bash
curl -X GET "https://localhost:5001/api/chatbot/history?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer $TOKEN"
```

### 5. Verify Token
```bash
curl -X GET https://localhost:5001/api/auth/verify \
  -H "Authorization: Bearer $TOKEN"
```

## Sample Query Responses

### Query: "What is the total turnover yearwise?"

**Request:**
```json
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
    "chatHistoryId": 5,
    "query": "What is the total turnover yearwise?",
    "response": "Query executed successfully.\n\nResults:\nThe company total turnover by year shows consistent growth.",
    "generatedSql": "SELECT c.Name as Company, YEAR(t.RecordDate) as Year, SUM(t.Amount) as TotalTurnover FROM Turnovers t JOIN Companies c ON t.CompanyId = c.Id GROUP BY c.Name, YEAR(t.RecordDate) ORDER BY c.Name, Year DESC",
    "isSuccessful": true,
    "data": {
      "TechCorp": {
        "2024": 6750000,
        "2023": 5350000
      },
      "FinanceHub": {
        "2024": 8750000,
        "2023": null
      },
      "RetailPlus": {
        "2024": 13500000,
        "2023": null
      }
    },
    "createdAt": "2024-06-29T10:30:45.123Z"
  }
}
```

## Error Responses

### Unauthorized (Missing Token)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Authorization header missing"
}
```

### Bad Request (Invalid Query)
```json
{
  "success": false,
  "message": "Query cannot be empty",
  "data": null
}
```

### Server Error
```json
{
  "success": false,
  "message": "An error occurred processing your request",
  "errors": {
    "error": "Detailed error message"
  }
}
```

## Performance Metrics

- **Average query response time:** 200-500ms
- **Login response time:** 100-150ms
- **Database query execution:** 50-100ms (depending on data volume)

## Rate Limiting (Future Enhancement)

Currently no rate limiting is implemented. In production, add:
- 100 requests per minute per user
- 1000 requests per minute per IP

## Testing Checklist

- [ ] API starts without errors
- [ ] Swagger UI is accessible
- [ ] Can register new user
- [ ] Can login with credentials
- [ ] JWT token is generated correctly
- [ ] Expired token is rejected
- [ ] Invalid token is rejected
- [ ] Chat query processes successfully
- [ ] SQL is generated correctly
- [ ] Results are returned in correct format
- [ ] Chat history is saved
- [ ] Companies list is retrieved
- [ ] Error handling returns proper status codes
- [ ] Logs are being written to file
- [ ] Database is being updated correctly

## Load Testing Script

```bash
#!/bin/bash

TOKEN="your_jwt_token_here"
BASE_URL="https://localhost:5001/api"

echo "Starting load test..."

for i in {1..100}; do
  curl -X POST $BASE_URL/chatbot/query \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d '{"query": "What is the total turnover yearwise?", "companyId": null}' \
    > /dev/null 2>&1 &
done

echo "100 requests sent"
```

## Database Verification

### Check if sample data is loaded:
```sql
SELECT COUNT(*) FROM Companies -- Should be 3
SELECT COUNT(*) FROM Turnovers -- Should be 16
SELECT COUNT(*) FROM Users -- Should be 1
SELECT COUNT(*) FROM ChatHistories -- Should increase with each query
```

### View user activity:
```sql
SELECT u.Username, COUNT(ch.Id) as QueryCount, MAX(ch.CreatedAt) as LastQuery
FROM Users u
LEFT JOIN ChatHistories ch ON u.Id = ch.UserId
GROUP BY u.Username
```

### Check query success rate:
```sql
SELECT 
  IsSuccessful,
  COUNT(*) as Count,
  CONVERT(DECIMAL(5,2), COUNT(*) * 100.0 / (SELECT COUNT(*) FROM ChatHistories)) as Percentage
FROM ChatHistories
GROUP BY IsSuccessful
```
