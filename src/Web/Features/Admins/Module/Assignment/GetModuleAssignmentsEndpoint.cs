using Application.Interfaces.Services.Module.Dto;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Module.Assignment;

public class GetModuleAssignmentsEndpoint : EndpointWithoutRequest<List<MemberModuleDto>>
{
    private readonly IMemberModuleRepository _repository;

    public GetModuleAssignmentsEndpoint(IMemberModuleRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/module/{moduleId}/assignments");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var moduleIdString = Route<string>("moduleId");
        if (!Guid.TryParse(moduleIdString, out var moduleId))
        {
            HttpContext.Response.StatusCode = 400;
            return;
        }

        var assignments = await _repository.GetByModuleIdAsync(moduleId);
        Response = assignments.Select(a => new MemberModuleDto
        {
            Id = a.Id.ToString(),
            MemberId = a.MemberId.ToString(),
            ModuleId = a.ModuleId.ToString(),
            MemberName = a.Member?.FullName
        }).ToList();
    }
}
