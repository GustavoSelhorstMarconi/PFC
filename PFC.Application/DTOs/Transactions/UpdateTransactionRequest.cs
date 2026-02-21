using PFC.Domain.Enums;

namespace PFC.Application.DTOs.Transactions;

public sealed class UpdateTransactionRequest
{
    public Guid AccountId { get; set; }
    public Guid CategoryId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
}
