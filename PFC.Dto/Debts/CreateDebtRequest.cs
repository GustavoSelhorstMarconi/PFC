namespace PFC.Dto.Debts;

public sealed class CreateDebtRequest
{
    public string Name { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public decimal? InterestRate { get; set; }
    public DateTime? DueDate { get; set; }
}
