using PFC.Application.Common;
using PFC.Application.Interfaces;
using PFC.Domain.Entities;
using PFC.Domain.Exceptions;
using PFC.Domain.Interfaces;
using PFC.Dto.Goals;

namespace PFC.Application.Services;

public sealed class GoalService : IGoalService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBaseRepository<Goal> _baseRepository;
    private readonly IGoalRepository _goalRepository;

    public GoalService(ICurrentUserService currentUserService, IBaseRepository<Goal> baseRepository, IGoalRepository goalRepository)
    {
        _currentUserService = currentUserService;
        _baseRepository = baseRepository;
        _goalRepository = goalRepository;
    }

    public async Task<Result<GoalResponse>> CreateGoalAsync(CreateGoalRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
            throw new BadRequestException("Invalid request");

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadRequestException("Name is required");

        if (request.TargetAmount <= 0)
            throw new BadRequestException("TargetAmount must be greater than zero");

        var userId = _currentUserService.GetUserId();

        var goal = new Goal(userId, request.Name, request.TargetAmount, request.Deadline);

        await _baseRepository.AddAsync(goal, cancellationToken);
        await _baseRepository.SaveChangesAsync(cancellationToken);

        var response = new GoalResponse
        {
            Id = goal.Id,
            Name = goal.Name,
            TargetAmount = goal.TargetAmount,
            CurrentAmount = goal.CurrentAmount,
            Deadline = goal.Deadline,
            IsActive = goal.IsActive,
            CreatedAt = goal.CreatedAt,
            UpdatedAt = goal.UpdatedAt
        };

        return Result.Success(response);
    }

    public async Task<Result<GoalResponse>> UpdateGoalAsync(Guid goalId, UpdateGoalRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
            throw new BadRequestException("Invalid request");

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadRequestException("Name is required");

        if (request.TargetAmount <= 0)
            throw new BadRequestException("TargetAmount must be greater than zero");

        var userId = _currentUserService.GetUserId();

        var goal = await _baseRepository.GetByIdAsync(goalId, cancellationToken);
        if (goal is null)
            throw new NotFoundException("Goal not found");

        if (goal.UserId != userId)
            throw new UnauthorizedException();

        goal.Update(request.Name, request.TargetAmount, request.Deadline);

        _baseRepository.Update(goal);
        await _baseRepository.SaveChangesAsync(cancellationToken);

        var response = new GoalResponse
        {
            Id = goal.Id,
            Name = goal.Name,
            TargetAmount = goal.TargetAmount,
            CurrentAmount = goal.CurrentAmount,
            Deadline = goal.Deadline,
            IsActive = goal.IsActive,
            CreatedAt = goal.CreatedAt,
            UpdatedAt = goal.UpdatedAt
        };

        return Result.Success(response);
    }

    public async Task<Result> DeactivateGoalAsync(Guid goalId, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var goal = await _baseRepository.GetByIdAsync(goalId, cancellationToken);
        if (goal is null)
            throw new NotFoundException("Goal not found");

        if (goal.UserId != userId)
            throw new UnauthorizedException();

        goal.Deactivate();

        _baseRepository.Update(goal);
        await _baseRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<IEnumerable<GoalResponse>>> GetUserGoalsAsync(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var goals = await _goalRepository.GetUserGoalsAsync(userId, cancellationToken);

        var result = goals.Select(g => new GoalResponse
        {
            Id = g.Id,
            Name = g.Name,
            TargetAmount = g.TargetAmount,
            CurrentAmount = g.CurrentAmount,
            Deadline = g.Deadline,
            IsActive = g.IsActive,
            CreatedAt = g.CreatedAt,
            UpdatedAt = g.UpdatedAt
        }).ToList();

        return Result.Success<IEnumerable<GoalResponse>>(result);
    }

    public async Task<Result> AddContributionAsync(Guid goalId, decimal amount, CancellationToken cancellationToken)
    {
        if (amount <= 0)
            throw new BadRequestException("Amount must be greater than zero");

        var userId = _currentUserService.GetUserId();

        var goal = await _baseRepository.GetByIdAsync(goalId, cancellationToken);
        if (goal is null)
            throw new NotFoundException("Goal not found");

        if (goal.UserId != userId)
            throw new UnauthorizedException();

        goal.AddContribution(amount);

        _baseRepository.Update(goal);
        await _baseRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
