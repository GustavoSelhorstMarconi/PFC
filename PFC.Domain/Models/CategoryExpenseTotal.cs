namespace PFC.Domain.Models;

public sealed class CategoryExpenseTotal
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string Color { get; set; } = null!;
    public decimal Total { get; set; }
}
