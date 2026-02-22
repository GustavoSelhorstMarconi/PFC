namespace PFC.Dto.Goals;

public sealed class GoalResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public DateTime? Deadline { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
