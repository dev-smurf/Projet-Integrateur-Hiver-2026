using Application.Interfaces.Services.Users;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Appointments;

public class GetAvailabilityEndpoint : EndpointWithoutRequest<object>
{
    private readonly IAdminAvailabilityRepository _availabilityRepository;
    private readonly IAuthenticatedUserService _authenticatedUserService;

    public GetAvailabilityEndpoint(
        IAdminAvailabilityRepository availabilityRepository,
        IAuthenticatedUserService authenticatedUserService)
    {
        _availabilityRepository = availabilityRepository;
        _authenticatedUserService = authenticatedUserService;
    }

    public override void Configure()
    {
        Get("appointments/availability");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var user = _authenticatedUserService.GetAuthenticatedUser();
        if (user == null)
        {
            HttpContext.Response.StatusCode = 401;
            return;
        }

        var recurring = await _availabilityRepository.GetByAdminIdAsync(user.Id);
        var overrides = await _availabilityRepository.GetOverridesByAdminIdAsync(user.Id);

        var result = new
        {
            Recurring = recurring.Select(r => new
            {
                r.Id,
                DayOfWeek = (int)r.DayOfWeek,
                StartTime = r.StartTime.ToString(@"hh\:mm"),
                EndTime = r.EndTime.ToString(@"hh\:mm")
            }),
            Overrides = overrides.Select(o => new
            {
                o.Id,
                Date = o.Date.Date,
                StartTime = o.StartTime?.ToString(@"hh\:mm"),
                EndTime = o.EndTime?.ToString(@"hh\:mm"),
                o.IsBlocked
            })
        };

        await Send.OkAsync(result, cancellation: ct);
    }
}
