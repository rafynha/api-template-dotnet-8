using System;
using component.template.domain.Interfaces.Infrastructure.Repository.Common;
using component.template.domain.Models.Repository;

namespace component.template.domain.Interfaces.Infrastructure.Repository;

public interface IUserRepository : IBaseRepository<UserDto> {}
