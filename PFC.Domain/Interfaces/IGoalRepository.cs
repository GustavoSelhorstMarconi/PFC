using PFC.Domain.Entities;

namespace PFC.Domain.Interfaces;

public interface IGoalRepository
{
    Task<IEnumerable<Goal>> GetUserGoalsAsync(Guid userId, CancellationToken cancellationToken);

    Task<IEnumerable<Goal>> GetActiveGoalsAsync(Guid userId, CancellationToken cancellationToken);
}
