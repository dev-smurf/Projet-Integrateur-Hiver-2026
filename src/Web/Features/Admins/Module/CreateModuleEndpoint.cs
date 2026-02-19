using AutoMapper;
using Domain.Common;
using Web.Features.Admins.Members.CreateMember;
using Web.Features.Common;

public class CreateModuleEndpoint : EndpointWithSanitizedRequest<CreateModulesRequest, SucceededOrNotResponse>
{
    private readonly IMapper _mapper; 
    
}   