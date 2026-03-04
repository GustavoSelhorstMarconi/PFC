using PFC.Domain.Enums;

namespace PFC.Dto.Transactions;

public sealed class TransactionResponse
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public string? AccountName { get; set; }
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public Guid? GoalId { get; set; }
    public Guid? DebtId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
