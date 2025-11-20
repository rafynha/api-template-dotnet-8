using component.template.domain.Interfaces.Infrastructure.Repository;
using component.template.domain.Models.Repository;
using component.template.domain.Models.Repository.Contexts;
using component.template.infrastructure.Repository.Common;
using Microsoft.EntityFrameworkCore;

namespace component.template.infrastructure.Repository.SqlServer;

public class UserRepository: BaseRepository<UserDto>, IUserRepository
{
    public UserRepository(SqlContext context) : base(context) { }
    
    // Construtor que aceita IDbContextFactory<SqlContext> e converte para DbContext via Adapter
    public UserRepository(SqlContext context, IDbContextFactory<SqlContext> contextFactory) 
        : base(context, new SqlContextFactoryAdapter(contextFactory)) { }    
}
