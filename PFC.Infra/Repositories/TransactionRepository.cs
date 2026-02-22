using Microsoft.EntityFrameworkCore;
using PFC.Domain.Entities;
using PFC.Domain.Enums;
using PFC.Domain.Interfaces;
using PFC.Domain.Models;
using PFC.Infra.Context;

namespace PFC.Infra.Repositories;

public sealed class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _context;

    public TransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AccountBalanceModel>> GetAccountBalancesByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var query = from a in _context.Accounts.AsNoTracking()
                    where a.UserId == userId && a.IsActive
                    join t in _context.Transactions.AsNoTracking().Where(t => t.IsActive) on a.Id equals t.AccountId into tx
                    select new AccountBalanceModel
                    {
                        AccountId = a.Id,
                        Name = a.Name,
                        InitialBalance = a.InitialBalance,
                        Income = tx.Where(x => x.Type == TransactionType.Income).Sum(x => (decimal?)x.Amount) ?? 0m,
                        Expense = tx.Where(x => x.Type == TransactionType.Expense).Sum(x => (decimal?)x.Amount) ?? 0m
                    };

        return await query.ToListAsync(cancellationToken);
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

    public async Task<(decimal Income, decimal Expense)> GetSumsByAccountAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var query = _context.Transactions
            .AsNoTracking()
            .Where(t => t.AccountId == accountId && t.IsActive);

        var income = await query.Where(t => t.Type == TransactionType.Income).SumAsync(t => (decimal?)t.Amount, cancellationToken) ?? 0m;
        var expense = await query.Where(t => t.Type == TransactionType.Expense).SumAsync(t => (decimal?)t.Amount, cancellationToken) ?? 0m;

        return (income, expense);
    }

    public async Task<(decimal TotalIncome, decimal TotalExpense)> GetTotalsByUserAsync(Guid userId, int? month, int? year, CancellationToken cancellationToken)
    {
        var query = _context
            .Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.IsActive);

        if (month.HasValue)
            query = query.Where(t => t.Date.Month == month.Value);

        if (year.HasValue)
            query = query.Where(t => t.Date.Year == year.Value);

        var totalIncome = await query
            .Where(t => t.Type == TransactionType.Income)
            .SumAsync(t => (decimal?)t.Amount, cancellationToken) ?? 0m;
        var totalExpense = await query
            .Where(t => t.Type == TransactionType.Expense)
            .SumAsync(t => (decimal?)t.Amount, cancellationToken) ?? 0m;

        return (totalIncome, totalExpense);
    }

    public async Task<IEnumerable<CategoryExpenseTotal>> GetExpenseTotalsByCategoryAsync(Guid userId, int month, int year, CancellationToken cancellationToken)
    {
        var query = from t in _context.Transactions.AsNoTracking()
                    join c in _context.Categories.AsNoTracking() on t.CategoryId equals c.Id
                    where t.UserId == userId && t.IsActive && t.Type == TransactionType.Expense && t.Date.Month == month && t.Date.Year == year
                    group t by new { c.Id, c.Name, c.Color } into g
                    select new CategoryExpenseTotal
                    {
                        CategoryId = g.Key.Id,
                        Name = g.Key.Name,
                        Color = g.Key.Color,
                        Total = g.Sum(x => x.Amount)
                    };

        return await query.ToListAsync(cancellationToken);
    }
}
