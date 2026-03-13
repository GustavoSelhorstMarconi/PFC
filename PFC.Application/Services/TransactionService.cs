using PFC.Application.Common;
using PFC.Application.Interfaces;
using PFC.Domain.Entities;
using PFC.Domain.Enums;
using PFC.Domain.Exceptions;
using PFC.Domain.Interfaces;
using PFC.Domain.Models;
using PFC.Dto.Common;
using PFC.Dto.Transactions;

namespace PFC.Application.Services;

public sealed class TransactionService : ITransactionService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBaseRepository<Transaction> _baseRepository;
    private readonly IBaseRepository<Account> _accountRepository;
    private readonly IBaseRepository<Category> _categoryRepository;
    private readonly IBaseRepository<Goal> _goalRepository;
    private readonly IBaseRepository<Debt> _debtRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IRecurrenceRepository _recurrenceRepository;

    public TransactionService(
        ICurrentUserService currentUserService,
        IBaseRepository<Transaction> baseRepository,
        IBaseRepository<Account> accountRepository,
        IBaseRepository<Category> categoryRepository,
        IBaseRepository<Goal> goalRepository,
        IBaseRepository<Debt> debtRepository,
        ITransactionRepository transactionRepository,
        IRecurrenceRepository recurrenceRepository
        )
    {
        _currentUserService = currentUserService;
        _baseRepository = baseRepository;
        _accountRepository = accountRepository;
        _categoryRepository = categoryRepository;
        _goalRepository = goalRepository;
        _debtRepository = debtRepository;
        _transactionRepository = transactionRepository;
        _recurrenceRepository = recurrenceRepository;
    }

    public async Task<Result<TransactionResponse>> CreateTransactionAsync(
    CreateTransactionRequest request,
    CancellationToken cancellationToken)
    {
        ValidateRequest(request);

        var userId = _currentUserService.GetUserId();

        await ValidateAccountAsync(request.AccountId, userId, cancellationToken);
        await ValidateCategoryAsync(request, userId, cancellationToken);

        var goal = await ValidateGoalAsync(request, userId, cancellationToken);
        var debt = await ValidateDebtAsync(request, userId, cancellationToken);

        var transaction = CreateTransactionEntity(request, userId);

        ApplyGoalContribution(goal, request.Amount);
        ApplyDebtPayment(debt, request.Amount);

        await _baseRepository.AddAsync(transaction, cancellationToken);
        await _baseRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(transaction));
    }

    private static void ValidateRequest(CreateTransactionRequest request)
    {
        if (request is null)
            throw new BadRequestException("Invalid request");

        if (request.Amount <= 0)
            throw new BadRequestException("Amount must be greater than zero");

        if (request.Description != null && request.Description.Length > 300)
            throw new BadRequestException("Description cannot exceed 300 characters");

        if (request.GoalId.HasValue && request.DebtId.HasValue)
            throw new BadRequestException("Transaction cannot be linked to both Goal and Debt");
    }

    private async Task ValidateAccountAsync(Guid accountId, Guid userId, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);

        if (account is null)
            throw new NotFoundException("Account not found");

        if (account.UserId != userId)
            throw new UnauthorizedException();
    }

    private async Task ValidateCategoryAsync(CreateTransactionRequest request, Guid userId, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
            throw new NotFoundException("Category not found");

        if (category.UserId is not null && category.UserId != userId)
            throw new UnauthorizedException();

        if (request.Type == TransactionType.Income && category.Type != CategoryType.Income)
            throw new BadRequestException("Category type must be Income for Income transactions");

        if (request.Type == TransactionType.Expense && category.Type != CategoryType.Expense)
            throw new BadRequestException("Category type must be Expense for Expense transactions");
    }

    private async Task<Goal?> ValidateGoalAsync(CreateTransactionRequest request, Guid userId, CancellationToken cancellationToken)
    {
        if (!request.GoalId.HasValue)
            return null;

        var goal = await _goalRepository.GetByIdAsync(request.GoalId.Value, cancellationToken);

        if (goal is null)
            throw new NotFoundException("Goal not found");

        if (goal.UserId != userId)
            throw new UnauthorizedException();

        if (!goal.IsActive)
            throw new BadRequestException("Goal is not active");

        if (goal.IsCompleted())
            throw new BadRequestException("Goal already completed");

        return goal;
    }

    private async Task<Debt?> ValidateDebtAsync(CreateTransactionRequest request, Guid userId, CancellationToken cancellationToken)
    {
        if (!request.DebtId.HasValue)
            return null;

        if (request.Type != TransactionType.Expense)
            throw new BadRequestException("Debt payments must be Expense transactions");

        var debt = await _debtRepository.GetByIdAsync(request.DebtId.Value, cancellationToken);

        if (debt is null)
            throw new NotFoundException("Debt not found");

        if (debt.UserId != userId)
            throw new UnauthorizedException();

        if (!debt.IsActive)
            throw new BadRequestException("Debt is not active");

        if (debt.IsPaid())
            throw new BadRequestException("Debt already paid");

        return debt;
    }

    private static Transaction CreateTransactionEntity(CreateTransactionRequest request, Guid userId)
    {
        return new Transaction(
            userId,
            request.AccountId,
            request.CategoryId,
            request.Type,
            request.Amount,
            request.Date,
            request.GoalId,
            request.DebtId,
            null,
            request.Description
        );
    }

    private static void ApplyGoalContribution(Goal? goal, decimal amount)
    {
        if (goal is null)
            return;

        goal.AddContribution(amount);
    }

    private static void ApplyDebtPayment(Debt? debt, decimal amount)
    {
        if (debt is null)
            return;

        debt.RegisterPayment(amount);
    }

    private static TransactionResponse MapToResponse(Transaction transaction)
    {
        return new TransactionResponse
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
            UpdatedAt = transaction.UpdatedAt,
            GoalId = transaction.GoalId,
            DebtId = transaction.DebtId
        };
    }

    public async Task<Result<TransactionResponse>> UpdateTransactionAsync(Guid transactionId, UpdateTransactionRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
            throw new BadRequestException("Invalid request");

        if (request.Amount <= 0)
            throw new BadRequestException("Amount must be greater than zero");

        if (request.Description != null && request.Description.Length > 300)
            throw new BadRequestException("Description cannot exceed 300 characters");

        if (request.GoalId.HasValue && request.DebtId.HasValue)
            throw new BadRequestException("Transaction cannot be linked to both Goal and Debt");

        var userId = _currentUserService.GetUserId();

        var transaction = await _baseRepository.GetByIdAsync(transactionId, cancellationToken);
        if (transaction is null)
            throw new NotFoundException("Transaction not found");

        if (transaction.UserId != userId)
            throw new UnauthorizedException();

        await HandleGoal(
            userId,
            transaction.Amount,
            request.Amount,
            transaction.GoalId,
            request.GoalId,
            cancellationToken);

        await HandleDebt(
            userId,
            transaction.Amount,
            request.Amount,
            transaction.DebtId,
            request.DebtId,
            request.Type,
            cancellationToken);

        transaction.Update(
            request.AccountId,
            request.CategoryId,
            request.Type,
            request.Amount,
            request.Date,
            request.Description,
            request.GoalId,
            request.DebtId
        );

        _baseRepository.Update(transaction);
        await _baseRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(transaction));
    }

    private async Task HandleGoal(Guid userId, decimal oldAmount, decimal requestAmount, Guid? oldGoalId, Guid? goalId, CancellationToken cancellationToken)
    {
        Goal? oldGoal = null;
        Goal? newGoal = null;

        if (oldGoalId.HasValue)
        {
            oldGoal = await _goalRepository.GetByIdAsync(oldGoalId.Value, cancellationToken);
        }

        if (goalId.HasValue)
        {
            newGoal = await _goalRepository.GetByIdAsync(goalId.Value, cancellationToken);

            if (newGoal is null)
                throw new NotFoundException("Goal not found");

            if (newGoal.UserId != userId)
                throw new UnauthorizedException();

            if (!newGoal.IsActive)
                throw new BadRequestException("Goal is not active");
        }

        if (oldGoalId != goalId)
        {
            // Remove da meta antiga
            if (oldGoal is not null)
                oldGoal.RemoveContribution(oldAmount);

            // Adiciona na nova
            if (newGoal is not null)
                newGoal.AddContribution(requestAmount);
        }
        else if (oldGoal is not null)
        {
            // Mesma meta, valor alterado
            oldGoal.RemoveContribution(oldAmount);
            newGoal ??= oldGoal;
            newGoal.AddContribution(requestAmount);
        }
    }

    private async Task HandleDebt(Guid userId, decimal oldAmount, decimal newAmount, Guid? oldDebtId, Guid? newDebtId, TransactionType newType, CancellationToken cancellationToken)
    {
        Debt? oldDebt = null;
        Debt? newDebt = null;

        if (oldDebtId.HasValue)
        {
            oldDebt = await _debtRepository.GetByIdAsync(oldDebtId.Value, cancellationToken);
        }

        if (newDebtId.HasValue)
        {
            if (newType != TransactionType.Expense)
                throw new BadRequestException("Debt payments must be Expense transactions");

            newDebt = await _debtRepository.GetByIdAsync(newDebtId.Value, cancellationToken);

            if (newDebt is null)
                throw new NotFoundException("Debt not found");

            if (newDebt.UserId != userId)
                throw new UnauthorizedException();

            if (!newDebt.IsActive)
                throw new BadRequestException("Debt is not active");

            if (newDebt.IsPaid())
                throw new BadRequestException("Debt already paid");
        }

        // ===============================
        // Troca de dívida
        // ===============================
        if (oldDebtId != newDebtId)
        {
            if (oldDebt is not null)
                oldDebt.ReversePayment(oldAmount);

            if (newDebt is not null)
                newDebt.RegisterPayment(newAmount);
        }
        // ===============================
        // Mesma dívida, valor alterado
        // ===============================
        else if (oldDebt is not null)
        {
            oldDebt.ReversePayment(oldAmount);
            oldDebt.RegisterPayment(newAmount);
        }
    }

    public async Task<Result> DeleteTransactionAsync(Guid transactionId, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var transaction = await _baseRepository.GetByIdAsync(transactionId, cancellationToken);
        if (transaction is null)
            throw new NotFoundException("Transaction not found");

        if (transaction.UserId != userId)
            throw new UnauthorizedException();

        await RevertGoalAsync(
            transaction.GoalId,
            transaction.Amount,
            cancellationToken);

        await RevertDebtAsync(
            transaction.DebtId,
            transaction.Amount,
            cancellationToken);

        transaction.Deactivate();

        _baseRepository.Update(transaction);
        await _baseRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task RevertGoalAsync(Guid? goalId, decimal amount, CancellationToken cancellationToken)
    {
        if (!goalId.HasValue)
            return;

        var goal = await _goalRepository.GetByIdAsync(goalId.Value, cancellationToken);

        if (goal is null)
            return;

        goal.RemoveContribution(amount);
    }

    private async Task RevertDebtAsync(Guid? debtId, decimal amount, CancellationToken cancellationToken)
    {
        if (!debtId.HasValue)
            return;

        var debt = await _debtRepository.GetByIdAsync(debtId.Value, cancellationToken);

        if (debt is null)
            return;

        debt.ReversePayment(amount);
    }

    public async Task<Result<IEnumerable<TransactionResponse>>> GetUserTransactionsAsync(int? month, int? year, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var transactions = await _transactionRepository.GetByUserIdAsync(userId, month, year, cancellationToken);

        var result = transactions.Select(t => MapToResponse(t)).ToList();

        return Result.Success<IEnumerable<TransactionResponse>>(result);
    }

    public async Task<Result<PagedResponse<TransactionResponse>>> GetUserTransactionsPagedAsync(int? month, int? year, PagedRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var (items, totalCount) = await _transactionRepository.GetByUserIdPagedAsync(userId, month, year, request, cancellationToken);

        var response = new PagedResponse<TransactionResponse>
        {
            Items = items.Select(t => new TransactionResponse
            {
                Id = t.Id,
                AccountId = t.AccountId,
                AccountName = t.Account?.Name,
                CategoryId = t.CategoryId,
                CategoryName = t.Category?.Name,
                Type = t.Type,
                Amount = t.Amount,
                Date = t.Date,
                Description = t.Description,
                IsActive = t.IsActive,
                GoalId = t.GoalId,
                DebtId = t.DebtId,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            }).ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return Result.Success(response);
    }

    public async Task<Result<IEnumerable<TransactionResponse>>> GenerateTransactionFromRecurrencesAsync(List<GenerateTransactionFromRecurrenceRequest> recurrencesRequest, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var recurrenceIds = recurrencesRequest
            .Select(r => r.RecurrenceId)
            .Distinct()
            .ToList();

        var transactionsToCreate = new List<Transaction>();

        var recurrences = await _recurrenceRepository.GetRecurrencesByIds(recurrenceIds, cancellationToken);

        var existingTransactions = await _transactionRepository.GetTransactionsByRecurrencesIds(recurrenceIds);

        foreach (var recurrenceRequest in recurrencesRequest)
        {
            var alreadyExists = existingTransactions.Any(t =>
                t.RecurrenceId == recurrenceRequest.RecurrenceId &&
                t.Date == recurrenceRequest.OccurrenceDate);

            if (alreadyExists)
                continue;

            var recurrence = recurrences.SingleOrDefault(r => r.Id == recurrenceRequest.RecurrenceId);

            var newTransaction = new Transaction(
                userId,
                recurrence.AccountId,
                recurrence.CategoryId,
                recurrence.Type,
                recurrence.Amount,
                recurrenceRequest.OccurrenceDate,
                null,
                null,
                recurrence.Id,
                recurrence.Description
            );

            transactionsToCreate.Add(newTransaction);
        }

        await _baseRepository.AddRangeAsync(transactionsToCreate, cancellationToken);
        await _baseRepository.SaveChangesAsync(cancellationToken);

        var response = transactionsToCreate.Select(t => MapToResponse(t));

        return Result.Success(response);
    }
}
