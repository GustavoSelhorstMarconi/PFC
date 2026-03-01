namespace PFC.Dto.Recurrences;

public class PendingRecurrenceOccurrenceDto
{
    public Guid RecurrenceId { get; set; }
    public string Description { get; set; }
    public DateOnly OccurrenceDate { get; set; }
    public decimal Amount { get; set; }
    public Guid AccountId { get; set; }
    public Guid CategoryId { get; set; }
}