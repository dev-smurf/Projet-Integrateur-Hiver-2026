using Application.Services.Module.Dto;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Features.Admins.Members.CreateMember;
using Web.Features.Common;

public class CreateModuleEndpoint : EndpointWithSanitizedRequest<CreateModulesRequest, SucceededOrNotResponse>
{
    private readonly IMapper _mapper;
    private readonly IModuleRepository _moduleService; 
    
    public CreateModuleEndpoint(IMapper mapper, IModuleRepository moduleRepository)
    {
        _mapper = mapper;
        _moduleService = moduleRepository;
    }

    public override void Configure()
    {
        DontCatchExceptions();
        Post("module");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CreateModulesRequest req, CancellationToken ct)
{
    var newModule = _mapper.Map<Domain.Entities.Module>(req);
    await _moduleService.Create(newModule);
    
}

}   