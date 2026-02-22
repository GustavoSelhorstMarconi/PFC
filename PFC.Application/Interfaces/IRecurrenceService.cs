using PFC.Application.Common;
using PFC.Dto.Recurrences;

namespace PFC.Application.Interfaces;

public interface IRecurrenceService
{
    Task<Result<RecurrenceResponse>> CreateRecurrenceAsync(CreateRecurrenceRequest request, CancellationToken cancellationToken);
    Task<Result<RecurrenceResponse>> UpdateRecurrenceAsync(Guid id, UpdateRecurrenceRequest request, CancellationToken cancellationToken);
    Task<Result> DeactivateRecurrenceAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<IEnumerable<RecurrenceResponse>>> GetUserRecurrencesAsync(CancellationToken cancellationToken);
    Task<Result<IEnumerable<RecurrenceProjectionDto>>> GetProjectedOccurrencesAsync(DateTime from, DateTime to, CancellationToken cancellationToken);
}
