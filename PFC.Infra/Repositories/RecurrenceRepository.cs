using Microsoft.EntityFrameworkCore;
using PFC.Domain.Entities;
using PFC.Domain.Interfaces;
using PFC.Domain.Models;
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

    public async Task<(IEnumerable<Recurrence> Items, int TotalCount)> GetByUserIdPagedAsync(Guid userId, PagedRequest request, CancellationToken cancellationToken)
    {
        var query = _context.Recurrences
            .Include(r => r.Category)
            .AsNoTracking()
            .Where(r => r.UserId == userId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(r =>
                (r.Description != null && r.Description.ToLower().Contains(search)) ||
                r.Category.Name.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = (request.SortBy?.ToLower(), request.SortDir?.ToLower()) switch
        {
            ("description", "desc") => query.OrderByDescending(r => r.Description),
            ("description", _) => query.OrderBy(r => r.Description),
            ("amount", "desc") => query.OrderByDescending(r => r.Amount),
            ("amount", _) => query.OrderBy(r => r.Amount),
            ("frequency", "desc") => query.OrderByDescending(r => r.Frequency),
            ("frequency", _) => query.OrderBy(r => r.Frequency),
            ("type", "desc") => query.OrderByDescending(r => r.Type),
            ("type", _) => query.OrderBy(r => r.Type),
            ("enddate", "asc") => query.OrderBy(r => r.EndDate),
            ("enddate", "desc") => query.OrderByDescending(r => r.EndDate),
            ("startdate", "asc") => query.OrderBy(r => r.StartDate),
            _ => query.OrderByDescending(r => r.StartDate),
        };

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
