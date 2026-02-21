using PFC.Application.Common;
using PFC.Application.DTOs.Transactions;

namespace PFC.Application.Interfaces;

public interface ITransactionService
{
    Task<Result<TransactionResponse>> CreateTransactionAsync(CreateTransactionRequest request, CancellationToken cancellationToken);
    Task<Result<TransactionResponse>> UpdateTransactionAsync(Guid transactionId, UpdateTransactionRequest request, CancellationToken cancellationToken);
    Task<Result> DeleteTransactionAsync(Guid transactionId, CancellationToken cancellationToken);
    Task<Result<IEnumerable<TransactionResponse>>> GetUserTransactionsAsync(int? month, int? year, CancellationToken cancellationToken);
}
