namespace PFC.Dto.Dashboard;

public sealed class MonthlySummaryDto
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Balance { get; set; }
    public List<CategoryExpenseDto> ExpenseByCategory { get; set; } = new List<CategoryExpenseDto>();
}
