using System;
using component.template.api.domain.Interfaces.Infrastructure.Repository.Common;
using component.template.api.domain.Models.Repository;

namespace component.template.api.domain.Interfaces.Infrastructure.Repository;

public interface IUserRepository : IBaseRepository<UserDto> {}
