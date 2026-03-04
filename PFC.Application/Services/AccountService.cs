using PFC.Application.Common;
using PFC.Application.Interfaces;
using PFC.Domain.Entities;
using PFC.Domain.Exceptions;
using PFC.Domain.Interfaces;
using PFC.Dto.Accounts;

namespace PFC.Application.Services;

public sealed class AccountService : IAccountService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBaseRepository<Account> _baseRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;

    public AccountService(
        ICurrentUserService currentUserService,
        IBaseRepository<Account> baseRepository,
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository)
    {
        _currentUserService = currentUserService;
        _baseRepository = baseRepository;
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<AccountResponse>> CreateAccountAsync(CreateAccountRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        if (request is null)
            throw new BadRequestException("Invalid request");

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadRequestException("Name is required");

        if (request.InitialBalance < 0)
            throw new BadRequestException("Initial balance cannot be negative");

        var account = new Account(userId, request.Name, request.Type, request.InitialBalance);

        await _baseRepository.AddAsync(account, cancellationToken);
        await _baseRepository.SaveChangesAsync(cancellationToken);

        var response = new AccountResponse
        {
            Id = account.Id,
            Name = account.Name,
            Type = account.Type,
            InitialBalance = account.InitialBalance,
            CurrentBalance = account.InitialBalance,
            IsActive = account.IsActive,
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.UpdatedAt
        };

        return Result.Success(response);
    }

    public async Task<Result<AccountResponse>> UpdateAccountAsync(Guid accountId, UpdateAccountRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        if (request is null)
            throw new BadRequestException("Invalid request");

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadRequestException("Name is required");

        var account = await _baseRepository.GetByIdAsync(accountId, cancellationToken);
        if (account is null)
            throw new NotFoundException("Account not found");

        if (account.UserId != userId)
            throw new UnauthorizedException();

        account.UpdateName(request.Name, request.Type, request.IsActive);

        _baseRepository.Update(account);

        await _baseRepository.SaveChangesAsync(cancellationToken);

        var (income, expense) = await _transactionRepository.GetSumsByAccountAsync(account.Id, cancellationToken);

        var response = new AccountResponse
        {
            Id = account.Id,
            Name = account.Name,
            Type = account.Type,
            InitialBalance = account.InitialBalance,
            CurrentBalance = account.InitialBalance + income - expense,
            IsActive = account.IsActive,
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.UpdatedAt
        };

        return Result.Success(response);
    }

    public async Task<Result<IEnumerable<AccountResponse>>> GetUserAccountsAsync(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var accounts = await _accountRepository.GetByUserIdAsync(userId, cancellationToken);
        var balances = await _transactionRepository.GetAccountBalancesByUserAsync(userId, cancellationToken);
        var balanceMap = balances.ToDictionary(b => b.AccountId);

        var result = accounts.Select(a =>
        {
            balanceMap.TryGetValue(a.Id, out var b);
            var currentBalance = a.InitialBalance + (b?.Income ?? 0m) - (b?.Expense ?? 0m);

            return new AccountResponse
            {
                Id = a.Id,
                Name = a.Name,
                Type = a.Type,
                InitialBalance = a.InitialBalance,
                CurrentBalance = currentBalance,
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            };
        }).ToList();

        return Result.Success<IEnumerable<AccountResponse>>(result);
    }
}
