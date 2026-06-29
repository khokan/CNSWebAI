using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using CNSWebAI.Core.DTOs;
using CNSWebAI.Core.Models;
using CNSWebAI.Infrastructure.Data.Repositories;
using CNSWebAI.Services.LLM;
using System.Security.Claims;

namespace CNSWebAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatbotController : ControllerBase
{
    private readonly IChatbotService _chatbotService;
    private readonly IRepository<ChatHistory> _chatHistoryRepository;
    private readonly ITurnoverRepository _turnoverRepository;
    private readonly IRepository<Company> _companyRepository;
    private readonly ILogger _logger;

    public ChatbotController(
        IChatbotService chatbotService,
        IRepository<ChatHistory> chatHistoryRepository,
        ITurnoverRepository turnoverRepository,
        IRepository<Company> companyRepository,
        ILogger logger)
    {
        _chatbotService = chatbotService;
        _chatHistoryRepository = chatHistoryRepository;
        _turnoverRepository = turnoverRepository;
        _companyRepository = companyRepository;
        _logger = logger;
    }

    /// <summary>
    /// Send a query to the chatbot
    /// </summary>
    [HttpPost("query")]
    [ProducesResponseType(typeof(ChatResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SendQuery([FromBody] ChatRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Query))
                return BadRequest(new { message = "Query cannot be empty" });

            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            if (userId == 0)
                return Unauthorized();

            _logger.Information("Chatbot query from user {UserId}: {Query}", userId, request.Query);

            // Process the query
            var chatResponse = await _chatbotService.ProcessQueryAsync(request.Query, request.CompanyId);

            if (chatResponse.IsSuccessful)
            {
                try
                {
                    // Execute the generated SQL query
                    var queryResult = await ExecuteQueryAsync(chatResponse.GeneratedSql, request.CompanyId);
                    chatResponse.Data = queryResult;

                    // Format the response
                    chatResponse.Response = await _chatbotService.FormatResponseAsync(
                        chatResponse.GeneratedSql, 
                        queryResult);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error executing SQL query");
                    chatResponse.IsSuccessful = false;
                    chatResponse.ErrorMessage = "Error executing query: " + ex.Message;
                }
            }

            // Save chat history
            var chatHistory = new ChatHistory
            {
                UserId = userId,
                UserQuery = request.Query,
                GeneratedSql = chatResponse.GeneratedSql,
                Response = chatResponse.Response,
                RawQueryResult = chatResponse.Data?.ToString(),
                IsSuccessful = chatResponse.IsSuccessful,
                ErrorMessage = chatResponse.ErrorMessage,
                CreatedAt = DateTime.UtcNow
            };

            var savedHistory = await _chatHistoryRepository.AddAsync(chatHistory);
            chatResponse.ChatHistoryId = savedHistory.Id;

            await _chatHistoryRepository.SaveChangesAsync();

            return Ok(new ApiResponse<ChatResponse>
            {
                Success = chatResponse.IsSuccessful,
                Message = chatResponse.IsSuccessful ? "Query processed successfully" : "Error processing query",
                Data = chatResponse
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error in SendQuery");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "Error processing request",
                Errors = new Dictionary<string, string> { { "error", ex.Message } }
            });
        }
    }

    /// <summary>
    /// Get chat history for current user
    /// </summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(IEnumerable<ChatHistory>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChatHistory([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            if (userId == 0)
                return Unauthorized();

            var allHistory = await _chatHistoryRepository.GetAllAsync();
            var userHistory = allHistory
                .Where(ch => ch.UserId == userId)
                .OrderByDescending(ch => ch.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new ApiResponse<IEnumerable<ChatHistory>>
            {
                Success = true,
                Message = "Chat history retrieved successfully",
                Data = userHistory
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error retrieving chat history");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "Error retrieving chat history"
            });
        }
    }

    /// <summary>
    /// Get available companies for querying
    /// </summary>
    [HttpGet("companies")]
    [ProducesResponseType(typeof(IEnumerable<Company>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCompanies()
    {
        try
        {
            var companies = await _companyRepository.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<Company>>
            {
                Success = true,
                Message = "Companies retrieved successfully",
                Data = companies
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error retrieving companies");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "Error retrieving companies"
            });
        }
    }

    private async Task<object?> ExecuteQueryAsync(string sql, int? companyId = null)
    {
        // Pattern-based query execution (simplified version)
        if (sql.Contains("YEAR(RecordDate)") && sql.Contains("GROUP BY YEAR"))
        {
            if (companyId.HasValue)
            {
                return await _turnoverRepository.GetYearlyTurnoverAsync(companyId.Value);
            }
            else
            {
                var allTurnovers = await _turnoverRepository.GetAllAsync();
                return allTurnovers
                    .GroupBy(t => t.Year)
                    .Select(g => new { Year = g.Key, TotalTurnover = g.Sum(t => t.Amount) })
                    .OrderByDescending(x => x.Year);
            }
        }
        else if (sql.Contains("Quarter"))
        {
            if (companyId.HasValue)
            {
                return await _turnoverRepository.GetByCompanyIdAsync(companyId.Value);
            }
        }
        else if (sql.Contains("Companies"))
        {
            return await _companyRepository.GetAllAsync();
        }

        // Default: return turnover data
        return await _turnoverRepository.GetAllAsync();
    }
}
