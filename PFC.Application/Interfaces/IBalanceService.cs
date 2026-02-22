using PFC.Application.Common;
using PFC.Dto.Balance;

namespace PFC.Application.Interfaces;

public interface IBalanceService
{
    Task<decimal> GetAccountBalanceAsync(Guid accountId, CancellationToken cancellationToken);
    Task<Result<TotalBalanceDto>> GetTotalBalanceAsync(CancellationToken cancellationToken);
    Task<Result<IEnumerable<AccountBalanceDto>>> GetAccountsBalancesAsync(CancellationToken cancellationToken);
}
