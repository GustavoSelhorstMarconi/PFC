using PFC.Application.Common;
using PFC.Dto.Debts;

namespace PFC.Application.Interfaces;

public interface IDebtService
{
    Task<Result<DebtResponse>> CreateDebtAsync(CreateDebtRequest request, CancellationToken cancellationToken);

    Task<Result<DebtResponse>> UpdateDebtAsync(Guid id, UpdateDebtRequest request, CancellationToken cancellationToken);

    Task<Result> DeactivateDebtAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<DebtResponse>> GetDebtByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<IEnumerable<DebtResponse>>> GetUserDebtsAsync(CancellationToken cancellationToken);
}
