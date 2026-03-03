namespace PFC.Dto.Dashboard;

public sealed class MonthlyIncomeExpenseResponse
{
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
}
