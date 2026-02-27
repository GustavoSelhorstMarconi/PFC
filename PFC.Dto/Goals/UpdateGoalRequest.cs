namespace PFC.Dto.Goals;

public sealed class UpdateGoalRequest
{
    public string Name { get; set; } = null!;
    public decimal TargetAmount { get; set; }
    public DateOnly? Deadline { get; set; }
    public bool IsActive { get; set; }
}
