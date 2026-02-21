using PFC.Domain.Entities;

namespace PFC.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetByUserIdAsync(Guid userId, int? month, int? year, CancellationToken cancellationToken);
}
