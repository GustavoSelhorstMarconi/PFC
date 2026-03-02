using PFC.Domain.Entities;

namespace PFC.Domain.Interfaces;

public interface IRecurrenceRepository
{
    Task<IEnumerable<Recurrence>> GetUserRecurrencesAsync(Guid userId, CancellationToken cancellationToken);
    Task<IEnumerable<Recurrence>> GetActiveRecurrencesAsync(Guid userId, CancellationToken cancellationToken);
    Task<IEnumerable<Recurrence>> GetRecurrencesByIds(List<Guid> recurrencesIds, CancellationToken cancellationToken);
}
