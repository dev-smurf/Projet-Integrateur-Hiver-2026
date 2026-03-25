using Application.Interfaces.Services.Users;
using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Features.Appointments.Requests;

namespace Web.Features.Appointments;

public class SaveAvailabilityEndpoint : Endpoint<SaveAvailabilityRequest, object>
{
    private readonly IAdminAvailabilityRepository _availabilityRepository;
    private readonly IAuthenticatedUserService _authenticatedUserService;

    public SaveAvailabilityEndpoint(
        IAdminAvailabilityRepository availabilityRepository,
        IAuthenticatedUserService authenticatedUserService)
    {
        _availabilityRepository = availabilityRepository;
        _authenticatedUserService = authenticatedUserService;
    }

    public override void Configure()
    {
        Put("appointments/availability");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(SaveAvailabilityRequest req, CancellationToken ct)
    {
        var user = _authenticatedUserService.GetAuthenticatedUser();
        if (user == null)
        {
            HttpContext.Response.StatusCode = 401;
            return;
        }

        var entities = req.Slots.Select(s => new AdminAvailability
        {
            AdminId = user.Id,
            DayOfWeek = s.DayOfWeek,
            StartTime = TimeSpan.Parse(s.StartTime),
            EndTime = TimeSpan.Parse(s.EndTime)
        });

        await _availabilityRepository.SaveRecurringAsync(user.Id, entities);

        await Send.OkAsync(new { succeeded = true }, cancellation: ct);
    }
}
