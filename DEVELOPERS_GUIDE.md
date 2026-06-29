# CNSWebAI - Developer's Guide

## Learning Path Through the Code

### Step 1: Understand the Architecture
1. Read `PROJECT_SUMMARY.md` - Overview
2. Review the solution structure
3. Understand the 4-layer architecture

### Step 2: Follow the Request Flow

#### Authentication Flow
```
LoginRequest → AuthController.Login()
            → AuthenticationService.LoginAsync()
            → UserRepository.GetByUsernameAsync()
            → BCrypt.Verify()
            → JwtTokenService.GenerateToken()
            → LoginResponse with JWT token
```

#### Chatbot Query Flow
```
ChatRequest (with JWT)
        → ChatbotController.SendQuery()
        → User validation
        → IChatbotService.ProcessQueryAsync()
        → SQL generation
        → Query execution
        → Result formatting
        → ChatHistory save
        → ChatResponse with data
```

### Step 3: Examine Key Files

#### Program.cs - Dependency Injection
Key sections:
- Service registration
- DbContext configuration
- JWT authentication setup
- CORS configuration
- Exception middleware
- Database migration

```csharp
// Key pattern to understand:
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
// This is dependency injection - enables loose coupling
```

#### ApplicationDbContext.cs - Database Schema
Key sections:
- Entity configurations
- Relationships
- Seed data
- Migrations

```csharp
// Key pattern to understand:
modelBuilder.Entity<Turnover>()
    .HasOne(t => t.Company)
    .WithMany(c => c.Turnovers)
    // This defines the one-to-many relationship
```

#### Repository Pattern
Key sections:
- Generic repository for CRUD
- Specialized repositories for complex queries

```csharp
// Usage pattern:
var user = await _userRepository.GetByUsernameAsync("admin");
// Clean, testable, and reusable
```

### Step 4: Understanding JWT Authentication

1. **Token Generation** (JwtTokenService.cs)
   ```csharp
   - Create claims (user info)
   - Sign with secret key
   - Set expiration
   - Return token string
   ```

2. **Token Validation** (Program.cs)
   ```csharp
   - Check signature
   - Validate issuer
   - Check audience
   - Verify expiration
   ```

3. **Using Token**
   ```bash
   Authorization: Bearer eyJhbGc...
   ```

### Step 5: Chatbot Query Generation

Location: `LocalLLMChatbotService.GenerateSqlFromQueryAsync()`

Pattern matching algorithm:
```csharp
if (query.Contains("turnover") && query.Contains("year"))
{
    // Generate SQL for yearly turnover
}
else if (query.Contains("quarterly"))
{
    // Generate SQL for quarterly data
}
```

To add new query types:
1. Add new `if` condition in `GenerateSqlFromQueryAsync`
2. Add corresponding execution logic in `ExecuteQueryAsync`
3. Test with API_TESTING.md examples

## Making Common Changes

### Adding a New Entity

1. **Create Model** in `CNSWebAI.Core/Models/`
   ```csharp
   public class Product
   {
       public int Id { get; set; }
       public string Name { get; set; }
       public decimal Price { get; set; }
   }
   ```

2. **Add DbSet** in `ApplicationDbContext.cs`
   ```csharp
   public DbSet<Product> Products { get; set; }
   ```

3. **Configure Entity** in `OnModelCreating()`
   ```csharp
   modelBuilder.Entity<Product>()
       .HasKey(p => p.Id);
   ```

4. **Create Migration**
   ```bash
   dotnet ef migrations add AddProduct --project ../CNSWebAI.Infrastructure
   dotnet ef database update --project ../CNSWebAI.Infrastructure
   ```

### Adding a New API Endpoint

1. **Create Controller** in `CNSWebAI.API/Controllers/`
   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   public class ProductController : ControllerBase
   {
       [HttpGet("{id}")]
       public async Task<IActionResult> GetProduct(int id) { }
   }
   ```

2. **Inject Dependencies**
   ```csharp
   public ProductController(IRepository<Product> repo, ILogger logger)
   {
       _repo = repo;
       _logger = logger;
   }
   ```

3. **Implement Logic**
   ```csharp
   public async Task<IActionResult> GetProduct(int id)
   {
       var product = await _repo.GetByIdAsync(id);
       return Ok(product);
   }
   ```

### Adding a New Chat Query Type

1. **Edit** `LocalLLMChatbotService.cs`

2. **Add pattern** in `GenerateSqlFromQueryAsync()`
   ```csharp
   else if (query.Contains("products") && query.Contains("price"))
   {
       sql = "SELECT * FROM Products WHERE Price > 0";
   }
   ```

3. **Add execution** in `ExecuteQueryAsync()`
   ```csharp
   else if (sql.Contains("Products"))
   {
       return await productRepository.GetAllAsync();
   }
   ```

4. **Test** using API_TESTING.md examples

### Adding Authentication to Endpoint

```csharp
[HttpGet("secure")]
[Authorize]  // Add this attribute
public IActionResult SecureEndpoint()
{
    var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
    return Ok(new { userId });
}
```

### Custom Error Handling

In controllers:
```csharp
try
{
    // Your code
}
catch (Exception ex)
{
    _logger.Error(ex, "Description of what failed");
    return StatusCode(500, new ApiResponse<object>
    {
        Success = false,
        Message = "Error message"
    });
}
```

## Testing Your Changes

### Unit Testing Pattern
```csharp
[TestClass]
public class UserRepositoryTests
{
    private ApplicationDbContext _context;
    private UserRepository _repository;

    [TestInitialize]
    public void Setup()
    {
        // Create in-memory database
        // Initialize repository
    }

    [TestMethod]
    public async Task GetByUsername_ReturnsUser()
    {
        // Arrange
        var username = "testuser";

        // Act
        var result = await _repository.GetByUsernameAsync(username);

        // Assert
        Assert.IsNotNull(result);
    }
}
```

### Integration Testing
Use Swagger UI or API_TESTING.md examples

## Performance Optimization Tips

### 1. Database Queries
```csharp
// ❌ Bad - N+1 problem
var companies = await _repo.GetAllAsync();
foreach (var company in companies)
{
    var turnovers = await _turnoverRepo.GetByCompanyIdAsync(company.Id);
}

// ✅ Good - Include related data
var companies = await _context.Companies
    .Include(c => c.Turnovers)
    .ToListAsync();
```

### 2. Caching
```csharp
// Add Redis caching
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// Use in service
var cached = await _cache.GetStringAsync("companies");
if (cached == null)
{
    cached = JsonConvert.SerializeObject(companies);
    await _cache.SetStringAsync("companies", cached);
}
```

### 3. Query Optimization
```csharp
// ❌ Slow - Select all then filter
var users = await _context.Users.ToListAsync();
var active = users.Where(u => u.IsActive).ToList();

// ✅ Fast - Filter at database
var active = await _context.Users
    .Where(u => u.IsActive)
    .ToListAsync();
```

## Debugging Tips

### 1. Enable Query Logging
In `Program.cs`:
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
           .LogTo(Console.WriteLine));
```

### 2. View Generated SQL
```csharp
var sql = _context.Turnovers.ToQueryString();
Console.WriteLine(sql);
```

### 3. Check Logs
```bash
Get-Content logs\app-*.txt | Select-String "ERROR"
```

## Common Issues & Solutions

### Issue: "Database does not exist"
**Solution:** Ensure connection string is correct, run migrations

### Issue: "JWT token invalid"
**Solution:** Check token hasn't expired, verify secret key matches

### Issue: "Null reference exception"
**Solution:** Add null checks, use [Authorize] attribute, check injected services

### Issue: "CORS error"
**Solution:** Check CORS policy in Program.cs, frontend URL must match

## Code Style Guidelines

```csharp
// ✅ Use
public class UserService { }
public async Task<User> GetUserAsync(int id) { }

// ❌ Avoid
public class user_service { }
public User GetUser(int id) { }

// ✅ Use meaningful names
var activeUsers = users.Where(u => u.IsActive);

// ❌ Avoid single letters (except in loops)
var a = users.Where(u => u.IsActive);

// ✅ Use async/await
return await repository.GetAsync();

// ❌ Avoid blocking
return repository.GetAsync().Result;
```

## Git Workflow

```bash
# Create feature branch
git checkout -b feature/add-products

# Make changes
# Commit with descriptive messages
git commit -m "Add ProductController and Product model"

# Push
git push origin feature/add-products

# Create pull request
```

## Deployment Checklist

- [ ] Update connection string for production
- [ ] Change JWT secret to production value
- [ ] Set environment to Production
- [ ] Disable Swagger in production
- [ ] Configure HTTPS certificates
- [ ] Set up monitoring/logging
- [ ] Run security scan
- [ ] Performance test
- [ ] Backup database
- [ ] Document API changes

## Resources for Learning

### C# & .NET
- Microsoft Docs: https://docs.microsoft.com/dotnet
- Entity Framework Core: https://docs.microsoft.com/ef/core

### Authentication
- JWT Best Practices: https://tools.ietf.org/html/rfc7519
- OWASP: https://owasp.org

### SQL Server
- SQL Server Docs: https://docs.microsoft.com/sql/sql-server
- Query Optimization: https://learn.microsoft.com/sql/relational-databases/query-processing-architecture

## Useful Commands

```bash
# Build solution
dotnet build

# Run tests
dotnet test

# Run API
dotnet run

# Create migration
dotnet ef migrations add Name --project ../CNSWebAI.Infrastructure

# Update database
dotnet ef database update --project ../CNSWebAI.Infrastructure

# View database schema
dotnet ef dbcontext info --project ../CNSWebAI.Infrastructure

# Clean build
dotnet clean && dotnet build

# Format code
dotnet format

# Analyze code
dotnet analyze
```

## File Structure for New Features

```
NewFeature/
├── Models/
│   └── NewEntity.cs
├── DTOs/
│   └── NewEntityDtos.cs
├── Controllers/
│   └── NewEntityController.cs
├── Repositories/
│   └── NewEntityRepository.cs
├── Services/
│   └── NewEntityService.cs
└── Tests/
    ├── NewEntityRepositoryTests.cs
    └── NewEntityControllerTests.cs
```

## Questions?

Refer to:
1. README.md - Overview
2. SETUP.md - Installation
3. API_TESTING.md - Testing
4. PROJECT_SUMMARY.md - Architecture
5. This file - Development

**Happy coding! 🚀**
