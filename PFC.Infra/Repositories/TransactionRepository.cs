using Microsoft.EntityFrameworkCore;
using PFC.Domain.Entities;
using PFC.Domain.Interfaces;
using PFC.Infra.Context;

namespace PFC.Infra.Repositories;

public sealed class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _context;

    public TransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Transaction>> GetByUserIdAsync(Guid userId, int? month, int? year, CancellationToken cancellationToken)
    {
        var query = _context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.IsActive);

        if (month.HasValue)
            query = query.Where(t => t.Date.Month == month.Value);

        if (year.HasValue)
            query = query.Where(t => t.Date.Year == year.Value);

        return await query.OrderByDescending(t => t.Date).ToListAsync(cancellationToken);
    }
}
