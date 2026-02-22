namespace PFC.Dto.Dashboard;

public sealed class CategoryExpenseDto
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string Color { get; set; } = null!;
    public decimal Total { get; set; }
}
