using PFC.Application.Common;
using PFC.Application.DTOs.Accounts;
using PFC.Application.Interfaces;
using PFC.Domain.Entities;
using PFC.Domain.Exceptions;
using PFC.Domain.Interfaces;

namespace PFC.Application.Services;

public sealed class AccountService : IAccountService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBaseRepository<Account> _baseRepository;
    private readonly IAccountRepository _accountRepository;

    public AccountService(ICurrentUserService currentUserService, IBaseRepository<Account> baseRepository, IAccountRepository accountRepository)
    {
        _currentUserService = currentUserService;
        _baseRepository = baseRepository;
        _accountRepository = accountRepository;
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

        account.UpdateName(request.Name);

        _baseRepository.Update(account);

        await _baseRepository.SaveChangesAsync(cancellationToken);

        var response = new AccountResponse
        {
            Id = account.Id,
            Name = account.Name,
            Type = account.Type,
            InitialBalance = account.InitialBalance,
            IsActive = account.IsActive,
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.UpdatedAt
        };

        return Result.Success(response);
    }

    public async Task<Result> DeactivateAccountAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var account = await _baseRepository.GetByIdAsync(accountId, cancellationToken);
        if (account is null)
            throw new NotFoundException("Account not found");

        if (account.UserId != userId)
            throw new UnauthorizedException();

        account.ChangeStatus(false);

        _baseRepository.Update(account);
        await _baseRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<IEnumerable<AccountResponse>>> GetUserAccountsAsync(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var accounts = await _accountRepository.GetByUserIdAsync(userId, cancellationToken);

        var result = accounts.Select(a => new AccountResponse
        {
            Id = a.Id,
            Name = a.Name,
            Type = a.Type,
            InitialBalance = a.InitialBalance,
            IsActive = a.IsActive,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt
        }).ToList();

        return Result.Success<IEnumerable<AccountResponse>>(result);
    }
}
