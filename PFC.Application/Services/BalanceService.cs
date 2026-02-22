using PFC.Application.Common;
using PFC.Application.Interfaces;
using PFC.Domain.Entities;
using PFC.Domain.Exceptions;
using PFC.Domain.Interfaces;
using PFC.Dto.Balance;

namespace PFC.Application.Services;

public sealed class BalanceService : IBalanceService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBaseRepository<Account> _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAccountRepository _accountRepoSpecific;

    public BalanceService(
        ICurrentUserService currentUserService,
        IBaseRepository<Account> accountRepository,
        ITransactionRepository transactionRepository,
        IAccountRepository accountRepoSpecific)
    {
        _currentUserService = currentUserService;
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _accountRepoSpecific = accountRepoSpecific;
    }

    public async Task<decimal> GetAccountBalanceAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account is null)
            throw new NotFoundException("Account not found");

        if (account.UserId != userId)
            throw new UnauthorizedException();

        var sums = await _transactionRepository.GetSumsByAccountAsync(accountId, cancellationToken);
        var balance = account.InitialBalance + sums.Income - sums.Expense;
        return balance;
    }

    public async Task<Result<TotalBalanceDto>> GetTotalBalanceAsync(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var initialSum = await _accountRepoSpecific.SumInitialBalancesByUserAsync(userId, cancellationToken);
        var totals = await _transactionRepository.GetTotalsByUserAsync(userId, null, null, cancellationToken);
        var balance = initialSum + totals.TotalIncome - totals.TotalExpense;

        return Result.Success(new TotalBalanceDto { Total = balance });
    }

    public async Task<Result<IEnumerable<AccountBalanceDto>>> GetAccountsBalancesAsync(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var models = await _transactionRepository.GetAccountBalancesByUserAsync(userId, cancellationToken);

        var dtos = models.Select(m => new AccountBalanceDto
        {
            AccountId = m.AccountId,
            Name = m.Name,
            InitialBalance = m.InitialBalance,
            Income = m.Income,
            Expense = m.Expense,
            Balance = m.InitialBalance + m.Income - m.Expense
        });

        return Result.Success(dtos);
    }
}
