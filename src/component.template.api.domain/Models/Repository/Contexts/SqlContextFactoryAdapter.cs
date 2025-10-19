using Microsoft.EntityFrameworkCore;

namespace component.template.api.domain.Models.Repository.Contexts;

public class SqlContextFactoryAdapter : IDbContextFactory<DbContext>
{
    private readonly IDbContextFactory<SqlContext> _sqlContextFactory;

    public SqlContextFactoryAdapter(IDbContextFactory<SqlContext> sqlContextFactory)
    {
        _sqlContextFactory = sqlContextFactory;
    }

    public DbContext CreateDbContext()
    {
        return _sqlContextFactory.CreateDbContext();
    }

    public async Task<DbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
    {
        return await _sqlContextFactory.CreateDbContextAsync(cancellationToken);
    }
}
