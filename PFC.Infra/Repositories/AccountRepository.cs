using Microsoft.EntityFrameworkCore;
using PFC.Domain.Entities;
using PFC.Domain.Enums;
using PFC.Domain.Interfaces;
using PFC.Infra.Context;

namespace PFC.Infra.Repositories;

public sealed class AccountRepository : IAccountRepository
{
    private readonly ApplicationDbContext _context;

    public AccountRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Accounts
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Account>> GetInvestmentAccountsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Accounts
            .AsNoTracking()
            .Where(a => a.UserId == userId && a.Type == AccountType.Investment)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> SumInitialBalancesByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Accounts
            .AsNoTracking()
            .Where(a => a.UserId == userId && a.IsActive)
            .SumAsync(a => (decimal?)a.InitialBalance, cancellationToken) ?? 0m;
    }
}
