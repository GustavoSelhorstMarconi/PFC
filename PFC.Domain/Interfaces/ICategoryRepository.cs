using PFC.Domain.Entities;

namespace PFC.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<Category?> GetByUserIdAndNameAsync(Guid userId, string name, CancellationToken cancellationToken);
}
