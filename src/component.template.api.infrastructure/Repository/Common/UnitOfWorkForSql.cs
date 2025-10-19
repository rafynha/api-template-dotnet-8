using component.template.api.domain.Interfaces.Infrastructure.Repository;
using component.template.api.domain.Interfaces.Infrastructure.Repository.Common;
using component.template.api.domain.Models.Repository.Contexts;

namespace component.template.api.infrastructure.Repository.Common;

public class UnitOfWorkForSql : IUnitOfWork
{
    private readonly SqlContext _context;
    private bool TransactionAlreadyOpen { get; set; }

    public IUserRepository Users { get; }

    // NOTA IMPORTANTE: 
    // O repositório injetado aqui tem acesso ao DbContextFactory, mas dentro de transações
    // ele NÃO deve usar o factory (pois criaria novos contextos fora da transação).
    // O BaseRepository detecta automaticamente: se tiver factory disponível, usa para FindPagedAsync.
    // Para operações CRUD normais (Add, Update, Remove), sempre usa o _context compartilhado.
    public UnitOfWorkForSql(SqlContext context,
                      IUserRepository users)
    {
        _context = context;
        Users = users;
    }

    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        if (_context.Database.CurrentTransaction == null)
        {
            await _context.Database.BeginTransactionAsync();
        }
        else
            TransactionAlreadyOpen = true;
    }

    public async Task CommitTransactionAsync()
    {
        var transaction = _context.Database.CurrentTransaction;
        if (transaction != null && !TransactionAlreadyOpen)
        {
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        else
            TransactionAlreadyOpen = false;
    }

    public async Task RollbackAsync()
    {
        var transaction = _context.Database.CurrentTransaction;
        if (transaction != null)
        {
            await transaction.RollbackAsync();
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}