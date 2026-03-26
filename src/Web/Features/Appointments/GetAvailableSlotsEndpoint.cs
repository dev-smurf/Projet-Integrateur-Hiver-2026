using Application.Interfaces.Services.Users;
using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Appointments;

public class GetAvailableSlotsRequest
{
    [QueryParam]
    public DateTime StartDate { get; set; }
    [QueryParam]
    public DateTime EndDate { get; set; }
}

public class GetAvailableSlotsEndpoint : Endpoint<GetAvailableSlotsRequest, object>
{
    private readonly IAdminAvailabilityRepository _availabilityRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IConversationRepository _conversationRepository;
    private readonly IAuthenticatedUserService _authenticatedUserService;

    public GetAvailableSlotsEndpoint(
        IAdminAvailabilityRepository availabilityRepository,
        IAppointmentRepository appointmentRepository,
        IConversationRepository conversationRepository,
        IAuthenticatedUserService authenticatedUserService)
    {
        _availabilityRepository = availabilityRepository;
        _appointmentRepository = appointmentRepository;
        _conversationRepository = conversationRepository;
        _authenticatedUserService = authenticatedUserService;
    }

    public override void Configure()
    {
        Get("appointments/available-slots");
        Roles(Domain.Constants.User.Roles.MEMBER);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(GetAvailableSlotsRequest req, CancellationToken ct)
    {
        var user = _authenticatedUserService.GetAuthenticatedUser();
        if (user == null)
        {
            HttpContext.Response.StatusCode = 401;
            return;
        }

        // Find admin for this member
        var conversation = await _conversationRepository.EnsureConversationForMemberAsync(user.Id);
        if (conversation == null)
        {
            await Send.OkAsync(Array.Empty<object>(), cancellation: ct);
            return;
        }

        var adminId = conversation.AdminId;

        var recurring = (await _availabilityRepository.GetByAdminIdAsync(adminId)).ToList();
        var overrides = (await _availabilityRepository.GetOverridesByAdminIdAsync(adminId)).ToList();

        var slots = new List<object>();
        var current = req.StartDate.Date;
        var end = req.EndDate.Date;

        while (current <= end)
        {
            // Check if there's an override for this date
            var dateOverrides = overrides.Where(o => o.Date.Date == current).ToList();

            if (dateOverrides.Any(o => o.IsBlocked))
            {
                current = current.AddDays(1);
                continue;
            }

            List<(TimeSpan Start, TimeSpan End)> timeRanges;

            if (dateOverrides.Any(o => !o.IsBlocked && o.StartTime.HasValue && o.EndTime.HasValue))
            {
                timeRanges = dateOverrides
                    .Where(o => !o.IsBlocked && o.StartTime.HasValue && o.EndTime.HasValue)
                    .Select(o => (o.StartTime!.Value, o.EndTime!.Value))
                    .ToList();
            }
            else
            {
                timeRanges = recurring
                    .Where(r => r.DayOfWeek == current.DayOfWeek)
                    .Select(r => (r.StartTime, r.EndTime))
                    .ToList();
            }

            foreach (var range in timeRanges)
            {
                var slotStart = range.Start;
                while (slotStart.Add(TimeSpan.FromHours(1)) <= range.End)
                {
                    var slotDateTime = current.Add(slotStart);

                    // Skip past dates
                    if (slotDateTime <= DateTime.UtcNow)
                    {
                        slotStart = slotStart.Add(TimeSpan.FromHours(1));
                        continue;
                    }

                    // Check for conflicts
                    var hasConflict = await _appointmentRepository.HasConflictAsync(adminId, slotDateTime);
                    if (!hasConflict)
                    {
                        slots.Add(new
                        {
                            Date = slotDateTime,
                            StartTime = slotStart.ToString(@"hh\:mm"),
                            EndTime = slotStart.Add(TimeSpan.FromHours(1)).ToString(@"hh\:mm")
                        });
                    }

                    slotStart = slotStart.Add(TimeSpan.FromHours(1));
                }
            }

            current = current.AddDays(1);
        }

        await Send.OkAsync(slots, cancellation: ct);
    }
}
