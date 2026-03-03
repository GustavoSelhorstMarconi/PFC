namespace PFC.Domain.Models;

public sealed class MonthlyIncomeExpenseModel
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
}
