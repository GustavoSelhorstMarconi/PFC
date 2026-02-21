namespace PFC.Domain.Interfaces;

public interface IBaseRepository<T> where T : class
{
    Task AddAsync(T entity, CancellationToken cancellationToken);

    Task AddRangeAsync(ICollection<T> entities, CancellationToken cancellationToken);

    void Update(T entity);

    void UpdateRange(ICollection<T> entities);

    void Delete(T entity);

    void DeleteRange(ICollection<T> entities);

    Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool asNoTracking = false);

    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken, bool asNoTracking = false);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}

