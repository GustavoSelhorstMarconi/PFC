using Microsoft.EntityFrameworkCore;
using PFC.Domain.Entities;
using PFC.Domain.Interfaces;
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
}
