using Domain.Helpers;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Persistence;
using Web.Dtos;

namespace Web.Features.Admins.Dashboard;

public class GetDashboardSummaryEndpoint : EndpointWithoutRequest<DashboardSummaryDto>
{
    private readonly GarneauTemplateDbContext _context;

    public GetDashboardSummaryEndpoint(GarneauTemplateDbContext context)
    {
        _context = context;
    }

    public override void Configure()
    {
        DontCatchExceptions();
        Get("admin/dashboard/summary");

        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task<DashboardSummaryDto> ExecuteAsync(CancellationToken ct)
    {
        var since = InstantHelper.GetLocalNow().Minus(Duration.FromDays(30));

        var totalMembers = await _context.Members.CountAsync(ct);
        var newMembers = await _context.Members.CountAsync(m => m.Created >= since, ct);
        var totalModules = await _context.Modules.CountAsync(ct);

        var totalMemberModules = await _context.MemberModules.CountAsync(ct);
        var completedMemberModules = await _context.MemberModules.CountAsync(m => m.IsCompleted, ct);
        var averageProgress = totalMemberModules == 0
            ? 0
            : (int)Math.Round(await _context.MemberModules.AverageAsync(m => m.ProgressPercent, ct));

        return new DashboardSummaryDto
        {
            TotalMembers = totalMembers,
            NewMembersLast30Days = newMembers,
            TotalModules = totalModules,
            TotalMemberModules = totalMemberModules,
            CompletedMemberModules = completedMemberModules,
            AverageProgressPercent = averageProgress
        };
    }
}
