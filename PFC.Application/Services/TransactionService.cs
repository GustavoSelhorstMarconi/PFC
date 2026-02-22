using PFC.Application.Common;
using PFC.Application.Interfaces;
using PFC.Domain.Entities;
using PFC.Domain.Enums;
using PFC.Domain.Exceptions;
using PFC.Domain.Interfaces;
using PFC.Dto.Transactions;

namespace PFC.Application.Services;

public sealed class TransactionService : ITransactionService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBaseRepository<Transaction> _baseRepository;
    private readonly IBaseRepository<Account> _accountRepository;
    private readonly IBaseRepository<Category> _categoryRepository;
    private readonly ITransactionRepository _transactionRepository;

    public TransactionService(
        ICurrentUserService currentUserService,
        IBaseRepository<Transaction> baseRepository,
        IBaseRepository<Account> accountRepository,
        IBaseRepository<Category> categoryRepository,
        ITransactionRepository transactionRepository
        )
    {
        _currentUserService = currentUserService;
        _baseRepository = baseRepository;
        _accountRepository = accountRepository;
        _categoryRepository = categoryRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<TransactionResponse>> CreateTransactionAsync(CreateTransactionRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
            throw new BadRequestException("Invalid request");

        if (request.Amount <= 0)
            throw new BadRequestException("Amount must be greater than zero");

        if (request.Description != null && request.Description.Length > 300)
            throw new BadRequestException("Description cannot exceed 300 characters");

        var userId = _currentUserService.GetUserId();

        var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (account is null)
            throw new NotFoundException("Account not found");

        if (account.UserId != userId)
            throw new UnauthorizedException();

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
            throw new NotFoundException("Category not found");

        if (category.UserId != userId)
            throw new UnauthorizedException();

        if (request.Type == TransactionType.Income && category.Type != CategoryType.Income)
            throw new BadRequestException("Category type must be Income for Income transactions");

        if (request.Type == TransactionType.Expense && category.Type != CategoryType.Expense)
            throw new BadRequestException("Category type must be Expense for Expense transactions");

        var transaction = new Transaction(userId, request.AccountId, request.CategoryId, request.Type, request.Amount, request.Date, request.Description);

        await _baseRepository.AddAsync(transaction, cancellationToken);
        await _baseRepository.SaveChangesAsync(cancellationToken);

        var response = new TransactionResponse
        {
            Id = transaction.Id,
            AccountId = transaction.AccountId,
            CategoryId = transaction.CategoryId,
            Type = transaction.Type,
            Amount = transaction.Amount,
            Date = transaction.Date,
            Description = transaction.Description,
            IsActive = transaction.IsActive,
            CreatedAt = transaction.CreatedAt,
            UpdatedAt = transaction.UpdatedAt
        };

        return Result.Success(response);
    }

    public async Task<Result<TransactionResponse>> UpdateTransactionAsync(Guid transactionId, UpdateTransactionRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
            throw new BadRequestException("Invalid request");

        if (request.Amount <= 0)
            throw new BadRequestException("Amount must be greater than zero");

        if (request.Description != null && request.Description.Length > 300)
            throw new BadRequestException("Description cannot exceed 300 characters");

        var userId = _currentUserService.GetUserId();

        var transaction = await _baseRepository.GetByIdAsync(transactionId, cancellationToken);
        if (transaction is null)
            throw new NotFoundException("Transaction not found");

        if (transaction.UserId != userId)
            throw new UnauthorizedException();

        var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (account is null)
            throw new NotFoundException("Account not found");

        if (account.UserId != userId)
            throw new UnauthorizedException();

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
            throw new NotFoundException("Category not found");

        if (category.UserId != userId)
            throw new UnauthorizedException();

        if (request.Type == TransactionType.Income && category.Type != CategoryType.Income)
            throw new BadRequestException("Category type must be Income for Income transactions");

        if (request.Type == TransactionType.Expense && category.Type != CategoryType.Expense)
            throw new BadRequestException("Category type must be Expense for Expense transactions");

        transaction.Update(request.AccountId, request.CategoryId, request.Type, request.Amount, request.Date, request.Description);

        _baseRepository.Update(transaction);
        await _baseRepository.SaveChangesAsync(cancellationToken);

        var response = new TransactionResponse
        {
            Id = transaction.Id,
            AccountId = transaction.AccountId,
            CategoryId = transaction.CategoryId,
            Type = transaction.Type,
            Amount = transaction.Amount,
            Date = transaction.Date,
            Description = transaction.Description,
            IsActive = transaction.IsActive,
            CreatedAt = transaction.CreatedAt,
            UpdatedAt = transaction.UpdatedAt
        };

        return Result.Success(response);
    }

    public async Task<Result> DeleteTransactionAsync(Guid transactionId, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var transaction = await _baseRepository.GetByIdAsync(transactionId, cancellationToken);
        if (transaction is null)
            throw new NotFoundException("Transaction not found");

        if (transaction.UserId != userId)
            throw new UnauthorizedException();

        transaction.Deactivate();

        _baseRepository.Update(transaction);
        await _baseRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<IEnumerable<TransactionResponse>>> GetUserTransactionsAsync(int? month, int? year, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var transactions = await _transactionRepository.GetByUserIdAsync(userId, month, year, cancellationToken);

        var result = transactions.Select(t => new TransactionResponse
        {
            Id = t.Id,
            AccountId = t.AccountId,
            CategoryId = t.CategoryId,
            Type = t.Type,
            Amount = t.Amount,
            Date = t.Date,
            Description = t.Description,
            IsActive = t.IsActive,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        }).ToList();

        return Result.Success<IEnumerable<TransactionResponse>>(result);
    }
}
