using PFC.Domain.Entities;

namespace PFC.Domain.Interfaces;

public interface IAccountRepository
{
    Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}
