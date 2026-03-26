using Domain.Common;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Module.Assignment;

public class UnassignModuleEndpoint : EndpointWithoutRequest<SucceededOrNotResponse>
{
    private readonly IMemberModuleRepository _repository;

    public UnassignModuleEndpoint(IMemberModuleRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Delete("/module/{moduleId}/assign/{memberId}");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var moduleIdString = Route<string>("moduleId");
        var memberIdString = Route<string>("memberId");

        if (!Guid.TryParse(moduleIdString, out var moduleId) ||
            !Guid.TryParse(memberIdString, out var memberId))
        {
            HttpContext.Response.StatusCode = 400;
            Response = new SucceededOrNotResponse(false,
                new Error { ErrorType = "InvalidId", ErrorMessage = "ID invalide." });
            return;
        }

        var assignment = await _repository.GetByMemberAndModuleAsync(memberId, moduleId);
        if (assignment is null)
        {
            HttpContext.Response.StatusCode = 404;
            Response = new SucceededOrNotResponse(false,
                new Error { ErrorType = "NotFound", ErrorMessage = "Assignation introuvable." });
            return;
        }

        await _repository.UnassignAsync(assignment);
        Response = new SucceededOrNotResponse(true);
    }
}
