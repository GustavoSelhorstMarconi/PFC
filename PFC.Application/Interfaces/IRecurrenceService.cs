using PFC.Application.Common;
using PFC.Dto.Recurrences;

namespace PFC.Application.Interfaces;

public interface IRecurrenceService
{
    Task<Result<RecurrenceResponse>> CreateRecurrenceAsync(CreateRecurrenceRequest request, CancellationToken cancellationToken);
    Task<Result<RecurrenceResponse>> UpdateRecurrenceAsync(Guid id, UpdateRecurrenceRequest request, CancellationToken cancellationToken);
    Task<Result<IEnumerable<RecurrenceResponse>>> GetUserRecurrencesAsync(CancellationToken cancellationToken);
    Task<Result<IEnumerable<RecurrenceProjectionDto>>> GetProjectedOccurrencesAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken);
    Task<Result<IEnumerable<PendingRecurrenceOccurrenceDto>>> GetPendingRecurrenceOccurrences(DateOnly untilDate, CancellationToken cancellationToken);
}
