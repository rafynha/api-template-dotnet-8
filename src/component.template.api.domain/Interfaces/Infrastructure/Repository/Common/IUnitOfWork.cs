
namespace component.template.api.domain.Interfaces.Infrastructure.Repository.Common;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    Task<int> CommitAsync();
    Task BeginTransactionAsync();
    Task RollbackAsync();
    Task CommitTransactionAsync();
}
