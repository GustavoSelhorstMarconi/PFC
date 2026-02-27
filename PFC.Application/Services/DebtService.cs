using PFC.Application.Common;
using PFC.Application.Interfaces;
using PFC.Domain.Entities;
using PFC.Domain.Exceptions;
using PFC.Domain.Interfaces;
using PFC.Dto.Debts;

namespace PFC.Application.Services;

public sealed class DebtService : IDebtService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBaseRepository<Debt> _baseRepository;
    private readonly IDebtRepository _debtRepository;

    public DebtService(ICurrentUserService currentUserService, IBaseRepository<Debt> baseRepository, IDebtRepository debtRepository)
    {
        _currentUserService = currentUserService;
        _baseRepository = baseRepository;
        _debtRepository = debtRepository;
    }

    public async Task<Result<DebtResponse>> CreateDebtAsync(CreateDebtRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
            throw new BadRequestException("Invalid request");

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadRequestException("Name is required");

        if (request.TotalAmount <= 0)
            throw new BadRequestException("TotalAmount must be greater than zero");

        var userId = _currentUserService.GetUserId();

        var debt = new Debt(userId, request.Name, request.TotalAmount, request.InterestRate, request.DueDate);

        await _baseRepository.AddAsync(debt, cancellationToken);
        await _baseRepository.SaveChangesAsync(cancellationToken);

        var response = new DebtResponse
        {
            Id = debt.Id,
            Name = debt.Name,
            TotalAmount = debt.TotalAmount,
            RemainingAmount = debt.RemainingAmount,
            InterestRate = debt.InterestRate,
            DueDate = debt.DueDate,
            IsActive = debt.IsActive,
            CreatedAt = debt.CreatedAt,
            UpdatedAt = debt.UpdatedAt
        };

        return Result.Success(response);
    }

    public async Task<Result<DebtResponse>> UpdateDebtAsync(Guid id, UpdateDebtRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
            throw new BadRequestException("Invalid request");

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadRequestException("Name is required");

        if (request.TotalAmount <= 0)
            throw new BadRequestException("TotalAmount must be greater than zero");

        var userId = _currentUserService.GetUserId();

        var debt = await _baseRepository.GetByIdAsync(id, cancellationToken);
        if (debt is null)
            throw new NotFoundException("Debt not found");

        if (debt.UserId != userId)
            throw new UnauthorizedException();

        if (debt.IsPaid())
            throw new BadRequestException("Cannot modify a paid debt");

        debt.Update(request.Name, request.TotalAmount, request.InterestRate, request.DueDate, request.IsActive);

        _baseRepository.Update(debt);
        await _baseRepository.SaveChangesAsync(cancellationToken);

        var response = new DebtResponse
        {
            Id = debt.Id,
            Name = debt.Name,
            TotalAmount = debt.TotalAmount,
            RemainingAmount = debt.RemainingAmount,
            InterestRate = debt.InterestRate,
            DueDate = debt.DueDate,
            IsActive = debt.IsActive,
            CreatedAt = debt.CreatedAt,
            UpdatedAt = debt.UpdatedAt
        };

        return Result.Success(response);
    }

    public async Task<Result<DebtResponse>> GetDebtByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var debt = await _baseRepository.GetByIdAsync(id, cancellationToken, asNoTracking: true);
        if (debt is null)
            throw new NotFoundException("Debt not found");

        if (debt.UserId != userId)
            throw new UnauthorizedException();

        var response = new DebtResponse
        {
            Id = debt.Id,
            Name = debt.Name,
            TotalAmount = debt.TotalAmount,
            RemainingAmount = debt.RemainingAmount,
            InterestRate = debt.InterestRate,
            DueDate = debt.DueDate,
            IsActive = debt.IsActive,
            CreatedAt = debt.CreatedAt,
            UpdatedAt = debt.UpdatedAt
        };

        return Result.Success(response);
    }

    public async Task<Result<IEnumerable<DebtResponse>>> GetUserDebtsAsync(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var debts = await _debtRepository.GetByUserAsync(userId, cancellationToken);

        var result = debts.Select(d => new DebtResponse
        {
            Id = d.Id,
            Name = d.Name,
            TotalAmount = d.TotalAmount,
            RemainingAmount = d.RemainingAmount,
            InterestRate = d.InterestRate,
            DueDate = d.DueDate,
            IsActive = d.IsActive,
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.UpdatedAt
        }).ToList();

        return Result.Success<IEnumerable<DebtResponse>>(result);
    }
}
