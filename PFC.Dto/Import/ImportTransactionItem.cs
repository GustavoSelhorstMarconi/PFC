using PFC.Domain.Enums;

namespace PFC.Dto.Import;

public sealed class ImportTransactionItem
{
    public string ExternalId { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid? SuggestedCategoryId { get; set; }
}
