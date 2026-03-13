using PFC.Domain.Entities;
using PFC.Domain.Models;

namespace PFC.Domain.Interfaces;

public interface IRecurrenceRepository
{
    Task<IEnumerable<Recurrence>> GetUserRecurrencesAsync(Guid userId, CancellationToken cancellationToken);
    Task<IEnumerable<Recurrence>> GetActiveRecurrencesAsync(Guid userId, CancellationToken cancellationToken);
    Task<IEnumerable<Recurrence>> GetRecurrencesByIds(List<Guid> recurrencesIds, CancellationToken cancellationToken);
    Task<(IEnumerable<Recurrence> Items, int TotalCount)> GetByUserIdPagedAsync(Guid userId, PagedRequest request, CancellationToken cancellationToken);
}
