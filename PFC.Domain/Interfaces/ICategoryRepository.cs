using PFC.Domain.Entities;
using PFC.Domain.Models;

namespace PFC.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<Category?> GetByUserIdAndNameAsync(Guid userId, string name, CancellationToken cancellationToken);
    Task<(IEnumerable<Category> Items, int TotalCount)> GetByUserIdPagedAsync(Guid userId, PagedRequest request, CancellationToken cancellationToken);
}
