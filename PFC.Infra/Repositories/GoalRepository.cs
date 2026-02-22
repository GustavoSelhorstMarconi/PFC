using Microsoft.EntityFrameworkCore;
using PFC.Domain.Entities;
using PFC.Domain.Interfaces;
using PFC.Infra.Context;

namespace PFC.Infra.Repositories;

public sealed class GoalRepository : IGoalRepository
{
    private readonly ApplicationDbContext _context;

    public GoalRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Goal>> GetUserGoalsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Goals
            .AsNoTracking()
            .Where(g => g.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Goal>> GetActiveGoalsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Goals
            .Where(g => g.UserId == userId && g.IsActive)
            .ToListAsync(cancellationToken);
    }
}
