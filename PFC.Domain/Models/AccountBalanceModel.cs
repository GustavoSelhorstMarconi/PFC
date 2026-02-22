namespace PFC.Domain.Models;

public sealed class AccountBalanceModel
{
    public Guid AccountId { get; set; }
    public string Name { get; set; } = null!;
    public decimal InitialBalance { get; set; }
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
}
