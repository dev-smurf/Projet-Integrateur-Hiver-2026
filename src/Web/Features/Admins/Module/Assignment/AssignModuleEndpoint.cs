using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Module.Assignment;

public class AssignModuleRequest
{
    public Guid MemberId { get; set; }
}

public class AssignModuleEndpoint : Endpoint<AssignModuleRequest, SucceededOrNotResponse>
{
    private readonly IMemberModuleRepository _repository;

    public AssignModuleEndpoint(IMemberModuleRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Post("/module/{moduleId}/assign");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(AssignModuleRequest req, CancellationToken ct)
    {
        var moduleIdString = Route<string>("moduleId");
        if (!Guid.TryParse(moduleIdString, out var moduleId))
        {
            HttpContext.Response.StatusCode = 400;
            Response = new SucceededOrNotResponse(false,
                new Error { ErrorType = "InvalidId", ErrorMessage = "ID invalide." });
            return;
        }

        if (await _repository.IsAssignedAsync(req.MemberId, moduleId))
        {
            Response = new SucceededOrNotResponse(false,
                new Error { ErrorType = "AlreadyAssigned", ErrorMessage = "Ce module est déjà assigné à ce membre." });
            return;
        }

        var memberModule = new MemberModule
        {
            MemberId = req.MemberId,
            ModuleId = moduleId
        };

        await _repository.AssignAsync(memberModule);
        Response = new SucceededOrNotResponse(true);
    }
}
