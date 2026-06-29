using Microsoft.EntityFrameworkCore;
using CNSWebAI.Core.Models;

namespace CNSWebAI.Infrastructure.Data.Repositories;

public interface ITurnoverRepository : IRepository<Turnover>
{
    Task<IEnumerable<Turnover>> GetByCompanyIdAsync(int companyId);
    Task<IEnumerable<Turnover>> GetByYearAsync(int year);
    Task<Dictionary<int, decimal>> GetYearlyTurnoverAsync(int companyId);
    Task<object> GetTurnoverByYearAndQuarterAsync(int companyId, int year, int quarter);
}

public class TurnoverRepository : Repository<Turnover>, ITurnoverRepository
{
    private readonly ApplicationDbContext _context;

    public TurnoverRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Turnover>> GetByCompanyIdAsync(int companyId)
    {
        return await _context.Turnovers
            .Where(t => t.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Turnover>> GetByYearAsync(int year)
    {
        return await _context.Turnovers
            .Where(t => t.Year == year)
            .ToListAsync();
    }

    public async Task<Dictionary<int, decimal>> GetYearlyTurnoverAsync(int companyId)
    {
        return await _context.Turnovers
            .Where(t => t.CompanyId == companyId)
            .GroupBy(t => t.Year)
            .Select(g => new { Year = g.Key, Total = g.Sum(t => t.Amount) })
            .ToDictionaryAsync(x => x.Year, x => x.Total);
    }

    public async Task<object> GetTurnoverByYearAndQuarterAsync(int companyId, int year, int quarter)
    {
        return await _context.Turnovers
            .Where(t => t.CompanyId == companyId && t.Year == year && t.Quarter == quarter)
            .Select(t => new { t.Id, t.Amount, t.Year, t.Quarter, t.Currency })
            .FirstOrDefaultAsync();
    }
}
