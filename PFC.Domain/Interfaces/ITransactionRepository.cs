using PFC.Domain.Entities;
using PFC.Domain.Models;

namespace PFC.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetByUserIdAsync(Guid userId, int? month, int? year, CancellationToken cancellationToken);
    Task<IEnumerable<AccountBalanceModel>> GetAccountBalancesByUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<(decimal Income, decimal Expense)> GetSumsByAccountAsync(Guid accountId, CancellationToken cancellationToken);
    Task<(decimal TotalIncome, decimal TotalExpense)> GetTotalsByUserAsync(Guid userId, int? month, int? year, CancellationToken cancellationToken);
    Task<IEnumerable<CategoryExpenseTotal>> GetExpenseTotalsByCategoryAsync(Guid userId, int month, int year, CancellationToken cancellationToken);
    Task<IEnumerable<TransactionByRecurrenceIdsResponse>> GetTransactionsByRecurrencesIds(List<Guid> recurrenceIds);
}
