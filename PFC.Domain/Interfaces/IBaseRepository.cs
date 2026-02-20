namespace PFC.Domain.Interfaces;

public interface IBaseRepository<T> where T : class
{
    Task AddAsync(T entity);

    Task AddRangeAsync(ICollection<T> entities);

    void Update(T entity);

    void UpdateRange(ICollection<T> entities);

    void Delete(T entity);

    void DeleteRange(ICollection<T> entities);

    Task<T> GetByIdAsync(int id, bool asNoTracking = false);

    Task<IEnumerable<T>> GetAllAsync(bool asNoTracking = false);

    Task SaveChangesAsync();
}

