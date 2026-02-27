namespace PFC.Dto.Debts;

public sealed class DebtResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal? InterestRate { get; set; }
    public DateOnly? DueDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
