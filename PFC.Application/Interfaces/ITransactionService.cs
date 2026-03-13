using PFC.Application.Common;
using PFC.Domain.Models;
using PFC.Dto.Common;
using PFC.Dto.Transactions;

namespace PFC.Application.Interfaces;

public interface ITransactionService
{
    Task<Result<TransactionResponse>> CreateTransactionAsync(CreateTransactionRequest request, CancellationToken cancellationToken);
    Task<Result<TransactionResponse>> UpdateTransactionAsync(Guid transactionId, UpdateTransactionRequest request, CancellationToken cancellationToken);
    Task<Result> DeleteTransactionAsync(Guid transactionId, CancellationToken cancellationToken);
    Task<Result<IEnumerable<TransactionResponse>>> GetUserTransactionsAsync(int? month, int? year, CancellationToken cancellationToken);
    Task<Result<IEnumerable<TransactionResponse>>> GenerateTransactionFromRecurrencesAsync(List<GenerateTransactionFromRecurrenceRequest> recurrences, CancellationToken cancellationToken);
    Task<Result<PagedResponse<TransactionResponse>>> GetUserTransactionsPagedAsync(int? month, int? year, PagedRequest request, CancellationToken cancellationToken);
}
