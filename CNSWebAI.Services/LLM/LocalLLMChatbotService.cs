using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CNSWebAI.Core.DTOs;

namespace CNSWebAI.Services.LLM;

public interface IChatbotService
{
    Task<ChatResponse> ProcessQueryAsync(string userQuery, int? companyId = null);
    Task<string> GenerateSqlFromQueryAsync(string userQuery, int? companyId = null);
    Task<string> FormatResponseAsync(string sqlQuery, object? queryResult);
}

public class LocalLLMChatbotService : IChatbotService
{
    private readonly ILogger _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public LocalLLMChatbotService(ILogger logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<ChatResponse> ProcessQueryAsync(string userQuery, int? companyId = null)
    {
        try
        {
            _logger.Information("Processing chatbot query: {Query}", userQuery);

            // Generate SQL from natural language query
            var generatedSql = await GenerateSqlFromQueryAsync(userQuery, companyId);

            var response = new ChatResponse
            {
                Query = userQuery,
                GeneratedSql = generatedSql,
                IsSuccessful = true,
                CreatedAt = DateTime.UtcNow
            };

            _logger.Information("Successfully generated SQL: {Sql}", generatedSql);
            return response;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error processing chatbot query: {Query}", userQuery);
            return new ChatResponse
            {
                Query = userQuery,
                IsSuccessful = false,
                ErrorMessage = ex.Message,
                CreatedAt = DateTime.UtcNow
            };
        }
    }

    public Task<string> GenerateSqlFromQueryAsync(string userQuery, int? companyId = null)
    {
        // This is a simplified local LLM implementation
        // In production, you would integrate with Ollama, LM Studio, or similar
        
        var query = userQuery.ToLower();
        var sql = "";

        try
        {
            // Pattern matching for common queries
            if (query.Contains("turnover") && query.Contains("year"))
            {
                if (companyId.HasValue)
                {
                    sql = $@"
                        SELECT YEAR(RecordDate) as Year, SUM(Amount) as TotalTurnover
                        FROM Turnovers
                        WHERE CompanyId = {companyId}
                        GROUP BY YEAR(RecordDate)
                        ORDER BY Year DESC";
                }
                else
                {
                    sql = @"
                        SELECT c.Name as Company, YEAR(t.RecordDate) as Year, SUM(t.Amount) as TotalTurnover
                        FROM Turnovers t
                        JOIN Companies c ON t.CompanyId = c.Id
                        GROUP BY c.Name, YEAR(t.RecordDate)
                        ORDER BY c.Name, Year DESC";
                }
            }
            else if (query.Contains("quarterly") || query.Contains("quarter"))
            {
                if (companyId.HasValue)
                {
                    sql = $@"
                        SELECT Year, Quarter, Amount, Currency
                        FROM Turnovers
                        WHERE CompanyId = {companyId}
                        ORDER BY Year DESC, Quarter DESC";
                }
                else
                {
                    sql = @"
                        SELECT c.Name as Company, t.Year, t.Quarter, t.Amount, t.Currency
                        FROM Turnovers t
                        JOIN Companies c ON t.CompanyId = c.Id
                        ORDER BY c.Name, t.Year DESC, t.Quarter DESC";
                }
            }
            else if (query.Contains("company") && query.Contains("total"))
            {
                sql = @"
                    SELECT c.Name, SUM(t.Amount) as TotalTurnover
                    FROM Companies c
                    LEFT JOIN Turnovers t ON c.Id = t.CompanyId
                    GROUP BY c.Name
                    ORDER BY TotalTurnover DESC";
            }
            else if (query.Contains("all companies") || query.Contains("list company"))
            {
                sql = @"
                    SELECT Id, Name, Code, Description, IsActive
                    FROM Companies
                    ORDER BY Name";
            }
            else if (query.Contains("recent") || query.Contains("latest"))
            {
                sql = @"
                    SELECT TOP 10 c.Name, t.Amount, t.Year, t.Quarter, t.RecordDate
                    FROM Turnovers t
                    JOIN Companies c ON t.CompanyId = c.Id
                    ORDER BY t.RecordDate DESC";
            }
            else
            {
                // Default: Get all turnover data
                sql = @"
                    SELECT c.Name as Company, t.Amount, t.Year, t.Quarter, t.Currency
                    FROM Turnovers t
                    JOIN Companies c ON t.CompanyId = c.Id
                    ORDER BY c.Name, t.Year DESC";
            }

            return Task.FromResult(sql.Trim());
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error generating SQL for query: {Query}", userQuery);
            throw;
        }
    }

    public Task<string> FormatResponseAsync(string sqlQuery, object? queryResult)
    {
        try
        {
            if (queryResult == null)
                return Task.FromResult("No results found for your query.");

            // Format the response based on query type
            var response = $"Query executed successfully.\n\nResults:\n{queryResult}";

            return Task.FromResult(response);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error formatting response");
            throw;
        }
    }
}
