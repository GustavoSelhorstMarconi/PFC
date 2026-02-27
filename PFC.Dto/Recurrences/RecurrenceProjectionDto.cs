using PFC.Domain.Enums;

namespace PFC.Dto.Recurrences;

public sealed class RecurrenceProjectionDto
{
    public DateOnly Date { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public string CategoryName { get; set; } = null!;
    public Guid AccountId { get; set; }
}
