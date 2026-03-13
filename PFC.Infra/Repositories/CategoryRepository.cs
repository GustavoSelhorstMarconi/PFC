using Microsoft.EntityFrameworkCore;
using PFC.Domain.Entities;
using PFC.Domain.Interfaces;
using PFC.Domain.Models;
using PFC.Infra.Context;

namespace PFC.Infra.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(c => c.UserId == userId || c.IsDefault)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetByUserIdAndNameAsync(Guid userId, string name, CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => (c.UserId == userId || c.IsDefault) && c.Name == name.Trim(), cancellationToken);
    }

    public async Task<(IEnumerable<Category> Items, int TotalCount)> GetByUserIdPagedAsync(Guid userId, PagedRequest request, CancellationToken cancellationToken)
    {
        var query = _context.Categories
            .AsNoTracking()
            .Where(c => c.UserId == userId || c.IsDefault);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = (request.SortBy?.ToLower(), request.SortDir?.ToLower()) switch
        {
            ("type", "desc") => query.OrderByDescending(c => c.Type),
            ("type", _) => query.OrderBy(c => c.Type),
            ("isactive", "desc") => query.OrderByDescending(c => c.IsActive),
            ("isactive", _) => query.OrderBy(c => c.IsActive),
            ("name", "desc") => query.OrderByDescending(c => c.Name),
            _ => query.OrderBy(c => c.Name),
        };

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
