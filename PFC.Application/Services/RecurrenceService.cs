using PFC.Application.Common;
using PFC.Application.Interfaces;
using PFC.Domain.Entities;
using PFC.Domain.Enums;
using PFC.Domain.Exceptions;
using PFC.Domain.Interfaces;
using PFC.Dto.Recurrences;

namespace PFC.Application.Services;

public sealed class RecurrenceService : IRecurrenceService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBaseRepository<Recurrence> _baseRepository;
    private readonly IBaseRepository<Account> _accountRepository;
    private readonly IBaseRepository<Category> _categoryRepository;
    private readonly IRecurrenceRepository _recurrenceRepository;
    private readonly ITransactionRepository _transactionRepository;

    public RecurrenceService(
        ICurrentUserService currentUserService,
        IBaseRepository<Recurrence> baseRepository,
        IBaseRepository<Account> accountRepository,
        IBaseRepository<Category> categoryRepository,
        IRecurrenceRepository recurrenceRepository,
        ITransactionRepository transactionRepository)
    {
        _currentUserService = currentUserService;
        _baseRepository = baseRepository;
        _accountRepository = accountRepository;
        _categoryRepository = categoryRepository;
        _recurrenceRepository = recurrenceRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<RecurrenceResponse>> CreateRecurrenceAsync(CreateRecurrenceRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
            throw new BadRequestException("Invalid request");

        if (request.Amount <= 0)
            throw new BadRequestException("Amount must be greater than zero");

        if (request.Interval < 1)
            throw new BadRequestException("Interval must be at least 1");

        if (request.EndDate.HasValue && request.EndDate.Value <= request.StartDate)
            throw new BadRequestException("EndDate must be greater than StartDate");

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
            throw new BadRequestException("Category type must be Income for Income recurrences");

        if (request.Type == TransactionType.Expense && category.Type != CategoryType.Expense)
            throw new BadRequestException("Category type must be Expense for Expense recurrences");

        var recurrence = new Recurrence(userId, request.AccountId, request.CategoryId, request.Type, request.Amount, request.Description,
            request.Frequency, request.Interval, request.StartDate, request.EndDate, request.GeneratesTransaction);

        await _baseRepository.AddAsync(recurrence, cancellationToken);
        await _baseRepository.SaveChangesAsync(cancellationToken);

        var response = MapToResponse(recurrence);

        return Result.Success(response);
    }

    public async Task<Result<RecurrenceResponse>> UpdateRecurrenceAsync(Guid id, UpdateRecurrenceRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
            throw new BadRequestException("Invalid request");

        if (request.Amount <= 0)
            throw new BadRequestException("Amount must be greater than zero");

        if (request.Interval < 1)
            throw new BadRequestException("Interval must be at least 1");

        if (request.EndDate.HasValue && request.EndDate.Value <= request.StartDate)
            throw new BadRequestException("EndDate must be greater than StartDate");

        if (request.Description != null && request.Description.Length > 300)
            throw new BadRequestException("Description cannot exceed 300 characters");

        var userId = _currentUserService.GetUserId();

        var recurrence = await _baseRepository.GetByIdAsync(id, cancellationToken);
        if (recurrence is null)
            throw new NotFoundException("Recurrence not found");

        if (recurrence.UserId != userId)
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
            throw new BadRequestException("Category type must be Income for Income recurrences");

        if (request.Type == TransactionType.Expense && category.Type != CategoryType.Expense)
            throw new BadRequestException("Category type must be Expense for Expense recurrences");

        recurrence.Update(request.AccountId, request.CategoryId, request.Type, request.Amount, request.Description,
            request.Frequency, request.Interval, request.IsActive, request.StartDate, request.EndDate, request.GeneratesTransaction);

        _baseRepository.Update(recurrence);
        await _baseRepository.SaveChangesAsync(cancellationToken);

        var response = MapToResponse(recurrence);

        return Result.Success(response);
    }

    public async Task<Result<IEnumerable<RecurrenceResponse>>> GetUserRecurrencesAsync(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var recurrences = await _recurrenceRepository.GetUserRecurrencesAsync(userId, cancellationToken);

        var result = recurrences.Select(r => MapToResponse(r)).ToList();

        return Result.Success<IEnumerable<RecurrenceResponse>>(result);
    }

    public async Task<Result<IEnumerable<RecurrenceProjectionDto>>> GetProjectedOccurrencesAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken)
    {
        if (from > to)
            throw new BadRequestException("From must be before To");

        var userId = _currentUserService.GetUserId();

        var recurrences = await _recurrenceRepository.GetActiveRecurrencesAsync(userId, cancellationToken);

        var projections = new List<RecurrenceProjectionDto>();

        foreach (var r in recurrences)
        {
            var start = r.StartDate;
            var end = r.EndDate;

            var current = start;

            while (current < from)
            {
                current = GetNextOccurrence(current, r.Frequency, r.Interval);

                if (end.HasValue && current > end.Value)
                    break;
            }

            while (current <= to)
            {
                if (end.HasValue && current > end.Value)
                    break;

                if (current >= from)
                {
                    projections.Add(new RecurrenceProjectionDto
                    {
                        Date = current,
                        Amount = r.Amount,
                        Type = r.Type,
                        CategoryName = r.Category.Name,
                        AccountId = r.AccountId
                    });
                }

                current = GetNextOccurrence(current, r.Frequency, r.Interval);
            }
        }

        return Result.Success<IEnumerable<RecurrenceProjectionDto>>(projections);
    }

    private DateOnly GetNextOccurrence(DateOnly current, RecurrenceFrequency frequency, int interval)
    {
        return frequency switch
        {
            RecurrenceFrequency.Daily => current.AddDays(interval),
            RecurrenceFrequency.Weekly => current.AddDays(7 * interval),
            RecurrenceFrequency.Monthly => current.AddMonths(interval),
            RecurrenceFrequency.Yearly => current.AddYears(interval),
            _ => throw new NotSupportedException("Unsupported frequency")
        };
    }

    private static RecurrenceResponse MapToResponse(Recurrence recurrence)
    {
        return new RecurrenceResponse
        {
            Id = recurrence.Id,
            AccountId = recurrence.AccountId,
            CategoryId = recurrence.CategoryId,
            Type = recurrence.Type,
            Amount = recurrence.Amount,
            Description = recurrence.Description,
            Frequency = recurrence.Frequency,
            Interval = recurrence.Interval,
            StartDate = recurrence.StartDate,
            EndDate = recurrence.EndDate,
            GeneratesTransaction = recurrence.GeneratesTransaction,
            IsActive = recurrence.IsActive,
            CreatedAt = recurrence.CreatedAt,
            UpdatedAt = recurrence.UpdatedAt
        };
    }

    public async Task<Result<IEnumerable<PendingRecurrenceOccurrenceDto>>> GetPendingRecurrenceOccurrences(DateOnly untilDate, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var recurrences = await _recurrenceRepository.GetActiveRecurrencesAsync(userId, cancellationToken);

        var recurrenceIds = recurrences.Select(r => r.Id).ToList();

        var generatedTransactions = await _transactionRepository.GetTransactionsByRecurrencesIds(recurrenceIds);

        var result = new List<PendingRecurrenceOccurrenceDto>();

        foreach (var recurrence in recurrences)
        {
            var nextDate = recurrence.StartDate;

            while (nextDate <= untilDate)
            {
                if (recurrence.EndDate.HasValue && nextDate > recurrence.EndDate)
                    break;

                var alreadyGenerated = generatedTransactions
                    .Any(t => t.RecurrenceId == recurrence.Id
                           && t.Date == nextDate);

                if (!alreadyGenerated)
                {
                    result.Add(new PendingRecurrenceOccurrenceDto
                    {
                        RecurrenceId = recurrence.Id,
                        Description = recurrence.Description,
                        OccurrenceDate = nextDate,
                        Amount = recurrence.Amount,
                        AccountId = recurrence.AccountId,
                        CategoryId = recurrence.CategoryId
                    });
                }

                nextDate = GetNextOccurrence(nextDate, recurrence.Frequency, recurrence.Interval);
            }
        }

        return result;
    }
}
