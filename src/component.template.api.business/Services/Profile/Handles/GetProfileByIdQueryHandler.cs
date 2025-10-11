using System;
using Aurora.Mediator;
using AutoMapper;
using component.template.api.domain.Common;
using component.template.api.domain.Exceptions;
using component.template.api.domain.Interfaces.Infrastructure.Repository.Common;
using component.template.api.domain.Models.Internal.Profile;
using component.template.api.domain.Models.Internal.Profile.Queries;

namespace component.template.api.business.Services.Profile.Handles;

public class GetProfileByIdQueryHandler : BaseHandler, IRequestHandler<GetProfileByIdQuery, GetProfileByIdResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProfileByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public override void ConfigureMappings(IMapperConfigurationExpression cfg)
    {
    }

    public async Task<GetProfileByIdResponse> Handle(GetProfileByIdQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(new GetProfileByIdResponse
        {
            Id = request.Id,
            Name = "Mocked Profile Name"
        });
    }
}
