#nullable enable
using System.Linq.Expressions;
using component.template.domain.Interfaces.Infrastructure.Repository.Common;
using Microsoft.EntityFrameworkCore;

namespace component.template.infrastructure.Repository.Common;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    #region Properties

    protected readonly DbContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly IDbContextFactory<DbContext>? _contextFactory;

    #endregion

    #region Constructors

    public BaseRepository(DbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
        _contextFactory = null;
    }

    public BaseRepository(DbContext context, IDbContextFactory<DbContext> contextFactory)
    {
        _context = context;
        _dbSet = _context.Set<T>();
        _contextFactory = contextFactory;
    }

    #endregion

    #region Public Methods
    public async Task<T?> GetByIdAsync(int id)
    {
        // Tenta obter o nome da chave primária dos metadados do EF
        var primaryKeyName = GetPrimaryKeyName();
        if (primaryKeyName != null)
        {
            // Usa a chave primária descoberta dinamicamente com AsNoTracking
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(e => EF.Property<int>(e, primaryKeyName) == id);
        }

        // Fallback: usa FindAsync (com tracking) se não conseguir descobrir a chave
        return await _dbSet.FindAsync(id);
    }

    public async Task<T?> GetByIdAsync(long id)
    {
        // Tenta obter o nome da chave primária dos metadados do EF
        var primaryKeyName = GetPrimaryKeyName();
        if (primaryKeyName != null)
        {
            // Usa a chave primária descoberta dinamicamente com AsNoTracking
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(e => EF.Property<long>(e, primaryKeyName) == id);
        }

        // Fallback: usa FindAsync (com tracking) se não conseguir descobrir a chave
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        query = query.AsNoTracking();
        if (includes != null)
        {
            query = query.AsSplitQuery();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        return await query.Where(predicate).ToListAsync();
    }

    public async Task<(IEnumerable<T> Data, int TotalCount)> FindPagedAsync(
        Expression<Func<T, bool>>? predicate,
        int pageNumber = 1,
        int pageSize = 10,
        Expression<Func<T, object>>? orderBy = null,
        bool orderByDescending = false,
        params Expression<Func<T, object>>[] includes)
    {
        ValidatePaginationParameters(ref pageNumber, ref pageSize);

        return _contextFactory != null
            ? await FindPagedWithSeparateContextsAsync(predicate, pageNumber, pageSize, orderBy, orderByDescending, includes)
            : await FindPagedWithSharedContextAsync(predicate, pageNumber, pageSize, orderBy, orderByDescending, includes);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AsNoTracking().Where(predicate).CountAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task<int> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return await _context.SaveChangesAsync();
    }

    public async Task<int> RemoveAsync(T entity)
    {
        _dbSet.Remove(entity);
        return await _context.SaveChangesAsync();
    }
    #endregion

    #region Private Helper Methods for FindPagedAsync

    private static void ValidatePaginationParameters(ref int pageNumber, ref int pageSize)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
    }

    private async Task<(IEnumerable<T> Data, int TotalCount)> FindPagedWithSeparateContextsAsync(
        Expression<Func<T, bool>>? predicate,
        int pageNumber,
        int pageSize,
        Expression<Func<T, object>>? orderBy,
        bool orderByDescending,
        Expression<Func<T, object>>[] includes)
    {
        await using var contextQuery = await _contextFactory!.CreateDbContextAsync();
        await using var contextCounter = await _contextFactory.CreateDbContextAsync();

        var countQuery = BuildCountQuery(contextCounter.Set<T>(), predicate);
        var dataQuery = BuildDataQuery(contextQuery.Set<T>(), predicate, orderBy, orderByDescending, includes);

        return await ExecutePagedQueriesAsync(countQuery, dataQuery, pageNumber, pageSize);
    }

    private async Task<(IEnumerable<T> Data, int TotalCount)> FindPagedWithSharedContextAsync(
        Expression<Func<T, bool>>? predicate,
        int pageNumber,
        int pageSize,
        Expression<Func<T, object>>? orderBy,
        bool orderByDescending,
        Expression<Func<T, object>>[] includes)
    {
        var countQuery = BuildCountQuery(_dbSet, predicate);
        var dataQuery = BuildDataQuery(_dbSet, predicate, orderBy, orderByDescending, includes);

        return await ExecutePagedQueriesAsync(countQuery, dataQuery, pageNumber, pageSize);
    }

    private static IQueryable<T> BuildCountQuery(DbSet<T> dbSet, Expression<Func<T, bool>>? predicate)
    {
        var query = dbSet.AsNoTracking().AsQueryable();

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return query;
    }

    private static IQueryable<T> BuildDataQuery(
        DbSet<T> dbSet,
        Expression<Func<T, bool>>? predicate,
        Expression<Func<T, object>>? orderBy,
        bool orderByDescending,
        Expression<Func<T, object>>[] includes)
    {
        var query = dbSet.AsNoTracking().AsQueryable();

        query = ApplyIncludes(query, includes);
        query = ApplyFilter(query, predicate);
        query = ApplyOrdering(query, orderBy, orderByDescending);

        return query;
    }

    private static IQueryable<T> ApplyIncludes(IQueryable<T> query, Expression<Func<T, object>>[] includes)
    {
        if (includes == null || includes.Length == 0)
            return query;

        query = query.AsSplitQuery();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return query;
    }

    private static IQueryable<T> ApplyFilter(IQueryable<T> query, Expression<Func<T, bool>>? predicate)
    {
        return predicate != null ? query.Where(predicate) : query;
    }

    private static IQueryable<T> ApplyOrdering(
        IQueryable<T> query,
        Expression<Func<T, object>>? orderBy,
        bool orderByDescending)
    {
        if (orderBy == null)
            return query;

        return orderByDescending
            ? query.OrderByDescending(orderBy)
            : query.OrderBy(orderBy);
    }

    private static async Task<(IEnumerable<T> Data, int TotalCount)> ExecutePagedQueriesAsync(
        IQueryable<T> countQuery,
        IQueryable<T> dataQuery,
        int pageNumber,
        int pageSize)
    {
        var countTask = countQuery.CountAsync();
        var dataTask = dataQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        await Task.WhenAll(countTask, dataTask);

        return (dataTask.Result, countTask.Result);
    }

    #endregion

    #region Private Helper Method for GetByIdAsync
    /// <summary>
    /// Obtém o nome da propriedade da chave primária através dos metadados do Entity Framework.
    /// Retorna null se não conseguir descobrir a chave primária.
    /// </summary>
    private string? GetPrimaryKeyName()
    {
        try
        {
            var entityType = _context.Model.FindEntityType(typeof(T));
            var primaryKey = entityType?.FindPrimaryKey();

            // Verifica se existe chave primária e se é simples (não composta)
            if (primaryKey?.Properties.Count == 1)
            {
                return primaryKey.Properties[0].Name;
            }

            return null; // Chave composta ou não encontrada
        }
        catch
        {
            return null; // Qualquer erro retorna null para usar fallback
        }
    }
    #endregion
}