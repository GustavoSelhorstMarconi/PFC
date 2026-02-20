using Microsoft.EntityFrameworkCore;
using PFC.Domain.Interfaces;
using PFC.Infra.Context;

namespace PFC.Infra.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public BaseRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task AddAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        await _dbSet.AddAsync(entity);
    }

    public async Task AddRangeAsync(ICollection<T> entities)
    {
        if (entities == null || entities.Count == 0)
            throw new ArgumentNullException(nameof(entities));

        await _dbSet.AddRangeAsync(entities);
    }

    public void Update(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public void UpdateRange(ICollection<T> entities)
    {
        if (entities == null || entities.Count == 0)
            throw new ArgumentNullException(nameof(entities));

        _dbSet.UpdateRange(entities);
    }

    public void Delete(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }

        _dbSet.Remove(entity);
    }

    public void DeleteRange(ICollection<T> entities)
    {
        if (entities == null || entities.Count == 0)
            throw new ArgumentNullException(nameof(entities));

        _dbSet.RemoveRange(entities);
    }

    public async Task<T> GetByIdAsync(int id, bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync(bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        return await _dbSet.ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
