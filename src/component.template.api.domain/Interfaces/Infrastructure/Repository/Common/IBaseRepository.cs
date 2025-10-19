#nullable enable
using System.Linq.Expressions;

namespace component.template.api.domain.Interfaces.Infrastructure.Repository.Common;

public interface IBaseRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<T> GetByIdAsync(long id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task<(IEnumerable<T> Data, int TotalCount)> FindPagedAsync(
        Expression<Func<T, bool>>? predicate, 
        int pageNumber, 
        int pageSize,
        Expression<Func<T, object>>? orderBy = null,
        bool orderByDescending = false,
        params Expression<Func<T, object>>[] includes);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task<int> UpdateAsync(T entity);
    Task<int> RemoveAsync(T entity);
}
