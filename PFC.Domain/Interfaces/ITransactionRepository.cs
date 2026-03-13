using PFC.Domain.Entities;
using PFC.Domain.Enums;
using PFC.Domain.Models;

namespace PFC.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetByUserIdAsync(Guid userId, int? month, int? year, CancellationToken cancellationToken);
    Task<(IEnumerable<Transaction> Items, int TotalCount)> GetByUserIdPagedAsync(Guid userId, int? month, int? year, PagedRequest request, CancellationToken cancellationToken);
    Task<IEnumerable<AccountBalanceModel>> GetAccountBalancesByUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<IEnumerable<AccountBalanceModel>> GetAccountBalancesByIdsAsync(List<Guid> ids, CancellationToken cancellationToken);
    Task<(decimal Income, decimal Expense)> GetSumsByAccountAsync(Guid accountId, CancellationToken cancellationToken);
    Task<(decimal TotalIncome, decimal TotalExpense)> GetTotalsByUserAsync(Guid userId, int? month, int? year, CancellationToken cancellationToken);
    Task<IEnumerable<CategoryExpenseTotal>> GetExpenseTotalsByCategoryAsync(Guid userId, int month, int year, CancellationToken cancellationToken);
    Task<IEnumerable<TransactionByRecurrenceIdsResponse>> GetTransactionsByRecurrencesIds(List<Guid> recurrenceIds);
    Task<IEnumerable<MonthlyIncomeExpenseModel>> GetMonthlyIncomeExpenseAsync(Guid userId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);
    Task<IEnumerable<CategoryExpenseTotal>> GetCategoryTotalsByRangeAsync(Guid userId, DateOnly fromDate, DateOnly toDate, List<TransactionType> types, CancellationToken cancellationToken);
    Task<IEnumerable<Transaction>> GetTransactionsByRangeDate(Guid userId, DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken);
    Task<IEnumerable<Transaction>> GetInvestmentTransactionsAsync(Guid userId, DateOnly toDate, CancellationToken cancellationToken);
}
