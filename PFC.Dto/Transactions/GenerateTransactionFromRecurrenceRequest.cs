namespace PFC.Dto.Transactions;

public class GenerateTransactionFromRecurrenceRequest
{
    public Guid RecurrenceId { get; set; }
    public DateOnly OccurrenceDate { get; set; }
}
