using PFC.Domain.Entities;

namespace PFC.Domain.Interfaces;

public interface IDebtRepository
{
    Task<List<Debt>> GetByUserAsync(Guid userId, CancellationToken cancellationToken);
}
