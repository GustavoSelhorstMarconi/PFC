namespace PFC.Dto.Dashboard;

public sealed class CategoryTotalsResponse
{
    public IEnumerable<CategoryTotalItemResponse> IncomeTotals { get; set; } = [];
    public IEnumerable<CategoryTotalItemResponse> ExpenseTotals { get; set; } = [];
}
