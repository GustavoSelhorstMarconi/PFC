namespace PFC.Domain.Models;

public class TransactionByRecurrenceIdsResponse
{
    public Guid RecurrenceId { get; set; }
    public DateOnly Date { get; set; }
}
