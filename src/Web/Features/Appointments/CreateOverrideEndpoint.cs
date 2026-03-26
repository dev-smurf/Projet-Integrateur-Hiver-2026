using Application.Interfaces.Services.Users;
using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Features.Appointments.Requests;

namespace Web.Features.Appointments;

public class CreateOverrideEndpoint : Endpoint<CreateOverrideRequest, object>
{
    private readonly IAdminAvailabilityRepository _availabilityRepository;
    private readonly IAuthenticatedUserService _authenticatedUserService;

    public CreateOverrideEndpoint(
        IAdminAvailabilityRepository availabilityRepository,
        IAuthenticatedUserService authenticatedUserService)
    {
        _availabilityRepository = availabilityRepository;
        _authenticatedUserService = authenticatedUserService;
    }

    public override void Configure()
    {
        Post("appointments/availability/overrides");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CreateOverrideRequest req, CancellationToken ct)
    {
        var user = _authenticatedUserService.GetAuthenticatedUser();
        if (user == null)
        {
            HttpContext.Response.StatusCode = 401;
            return;
        }

        var entity = new AdminAvailabilityOverride
        {
            AdminId = user.Id,
            Date = req.Date.Date,
            StartTime = req.StartTime != null ? TimeSpan.Parse(req.StartTime) : null,
            EndTime = req.EndTime != null ? TimeSpan.Parse(req.EndTime) : null,
            IsBlocked = req.IsBlocked
        };

        await _availabilityRepository.CreateOverrideAsync(entity);

        await Send.OkAsync(new
        {
            entity.Id,
            Date = entity.Date,
            StartTime = entity.StartTime?.ToString(@"hh\:mm"),
            EndTime = entity.EndTime?.ToString(@"hh\:mm"),
            entity.IsBlocked
        }, cancellation: ct);
    }
}
