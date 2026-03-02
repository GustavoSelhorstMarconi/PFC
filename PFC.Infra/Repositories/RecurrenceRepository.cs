using Microsoft.EntityFrameworkCore;
using PFC.Domain.Entities;
using PFC.Domain.Interfaces;
using PFC.Infra.Context;

namespace PFC.Infra.Repositories;

public sealed class RecurrenceRepository : IRecurrenceRepository
{
    private readonly ApplicationDbContext _context;

    public RecurrenceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Recurrence>> GetUserRecurrencesAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Recurrences
            .Include(r => r.Category)
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Recurrence>> GetActiveRecurrencesAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Recurrences
            .Include(r => r.Category)
            .Where(r => r.UserId == userId && r.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Recurrence>> GetRecurrencesByIds(List<Guid> recurrencesIds, CancellationToken cancellationToken)
    {
        return await _context.Recurrences
            .Where(r => recurrencesIds.Contains(r.Id))
            .ToListAsync(cancellationToken);
    }
}
