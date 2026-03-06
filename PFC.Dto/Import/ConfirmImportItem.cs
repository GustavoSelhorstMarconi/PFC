using PFC.Domain.Enums;

namespace PFC.Dto.Import;

public sealed class ConfirmImportItem
{
    public string ExternalId { get; set; } = string.Empty;
    public Guid AccountId { get; set; }
    public Guid CategoryId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public string? Description { get; set; }
}
