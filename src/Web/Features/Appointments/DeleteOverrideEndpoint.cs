using Application.Interfaces.Services.Users;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Appointments;

public class DeleteOverrideRequest
{
    public Guid Id { get; set; }
}

public class DeleteOverrideEndpoint : Endpoint<DeleteOverrideRequest, object>
{
    private readonly IAdminAvailabilityRepository _availabilityRepository;
    private readonly IAuthenticatedUserService _authenticatedUserService;

    public DeleteOverrideEndpoint(
        IAdminAvailabilityRepository availabilityRepository,
        IAuthenticatedUserService authenticatedUserService)
    {
        _availabilityRepository = availabilityRepository;
        _authenticatedUserService = authenticatedUserService;
    }

    public override void Configure()
    {
        Delete("appointments/availability/overrides/{Id}");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(DeleteOverrideRequest req, CancellationToken ct)
    {
        var user = _authenticatedUserService.GetAuthenticatedUser();
        if (user == null)
        {
            HttpContext.Response.StatusCode = 401;
            return;
        }

        await _availabilityRepository.DeleteOverrideAsync(req.Id);
        await Send.OkAsync(new { succeeded = true }, cancellation: ct);
    }
}
