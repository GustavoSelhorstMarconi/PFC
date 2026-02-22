namespace PFC.Dto.Goals;

public sealed class CreateGoalRequest
{
    public string Name { get; set; } = null!;
    public decimal TargetAmount { get; set; }
    public DateTime? Deadline { get; set; }
}
