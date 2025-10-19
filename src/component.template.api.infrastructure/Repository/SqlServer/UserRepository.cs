using component.template.api.domain.Interfaces.Infrastructure.Repository;
using component.template.api.domain.Models.Repository;
using component.template.api.domain.Models.Repository.Contexts;
using component.template.api.infrastructure.Repository.Common;
using Microsoft.EntityFrameworkCore;

namespace component.template.api.infrastructure.Repository.SqlServer;

public class UserRepository: BaseRepository<UserDto>, IUserRepository
{
    public UserRepository(SqlContext context) : base(context) { }
    
    public UserRepository(SqlContext context, IDbContextFactory<DbContext> contextFactory) 
        : base(context, contextFactory) { }
}
