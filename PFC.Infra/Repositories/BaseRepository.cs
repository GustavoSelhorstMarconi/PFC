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

    public async Task AddAsync(T entity, CancellationToken cancellationToken)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(ICollection<T> entities, CancellationToken cancellationToken)
    {
        if (entities == null || entities.Count == 0)
            throw new ArgumentNullException(nameof(entities));

        await _dbSet.AddRangeAsync(entities, cancellationToken);
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

    public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id, cancellationToken);
        }

        return await _dbSet.FindAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken, bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
        }

        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
