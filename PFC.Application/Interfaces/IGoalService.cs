using PFC.Application.Common;
using PFC.Dto.Goals;

namespace PFC.Application.Interfaces;

public interface IGoalService
{
    Task<Result<GoalResponse>> CreateGoalAsync(CreateGoalRequest request, CancellationToken cancellationToken);

    Task<Result<GoalResponse>> UpdateGoalAsync(Guid goalId, UpdateGoalRequest request, CancellationToken cancellationToken);

    Task<Result<IEnumerable<GoalResponse>>> GetUserGoalsAsync(CancellationToken cancellationToken);

    Task<Result> AddContributionAsync(Guid goalId, decimal amount, CancellationToken cancellationToken);
}
