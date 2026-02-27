using Microsoft.EntityFrameworkCore;
using PFC.Domain.Entities;
using PFC.Domain.Interfaces;
using PFC.Infra.Context;

namespace PFC.Infra.Repositories;

public class DebtRepository : IDebtRepository
{
    private readonly ApplicationDbContext _context;

    public DebtRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Debt>> GetByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Debts
            .AsNoTracking()
            .Where(g => g.UserId == userId)
            .ToListAsync(cancellationToken);
    }
}
