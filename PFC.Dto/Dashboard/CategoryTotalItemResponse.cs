namespace PFC.Dto.Dashboard;

public sealed class CategoryTotalItemResponse
{
    public string CategoryName { get; set; } = null!;
    public string Color { get; set; } = null!;
    public decimal Total { get; set; }
}
