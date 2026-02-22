using PFC.Application.Common;
using PFC.Dto.Accounts;

namespace PFC.Application.Interfaces;

public interface IAccountService
{
    Task<Result<AccountResponse>> CreateAccountAsync(CreateAccountRequest request, CancellationToken cancellationToken);
    Task<Result<AccountResponse>> UpdateAccountAsync(Guid accountId, UpdateAccountRequest request, CancellationToken cancellationToken);
    Task<Result> DeactivateAccountAsync(Guid accountId, CancellationToken cancellationToken);
    Task<Result<IEnumerable<AccountResponse>>> GetUserAccountsAsync(CancellationToken cancellationToken);
}
