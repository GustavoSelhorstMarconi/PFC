namespace PFC.Dto.Balance;

public sealed class AccountBalanceDto
{
    public Guid AccountId { get; set; }
    public string Name { get; set; } = null!;
    public decimal InitialBalance { get; set; }
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
    public decimal Balance { get; set; }
}
