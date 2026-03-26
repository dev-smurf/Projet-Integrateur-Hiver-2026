using Domain.Entities;
using Domain.Helpers;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Persistence;
using Web.Dtos;

namespace Web.Features.Admins.Members.GetRecentMembers;

public class GetRecentMembersEndpoint : Endpoint<GetRecentMembersRequest, List<MemberDto>>
{
    private readonly GarneauTemplateDbContext _context;

    public GetRecentMembersEndpoint(GarneauTemplateDbContext context)
    {
        _context = context;
    }

    public override void Configure()
    {
        DontCatchExceptions();
        Get("admin/members/recent");

        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(GetRecentMembersRequest req, CancellationToken ct)
    {
        var days = req.Days <= 0 ? 30 : req.Days;
        var pageSize = req.PageSize <= 0 ? 50 : req.PageSize;
        var since = InstantHelper.GetLocalNow().Minus(Duration.FromDays(days));

        var query = _context.Members
            .Include(m => m.User)
            .ThenInclude(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Where(m => m.Created >= since)
            .OrderByDescending(m => m.Created)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(req.SearchValue))
        {
            var search = req.SearchValue.Trim().ToLower();
            query = query.Where(m =>
                m.FirstName.ToLower().Contains(search) ||
                m.LastName.ToLower().Contains(search) ||
                m.User.Email!.ToLower().Contains(search) ||
                (m.City != null && m.City.ToLower().Contains(search)));
        }

        var members = await query.Take(pageSize).ToListAsync(ct);
        var result = members.Select(MapMember).ToList();

        await Send.OkAsync(result, ct);
    }

    private static MemberDto MapMember(Member member)
    {
        var roles = member.User.UserRoles.Select(r => r.Role.Name ?? string.Empty).Where(r => !string.IsNullOrWhiteSpace(r)).ToList();
        return new MemberDto
        {
            Id = member.Id,
            UserId = member.User.Id,
            Created = member.Created.ToDateTimeUtc(),
            Active = member.Active,
            FirstName = member.FirstName,
            LastName = member.LastName,
            FullName = member.FullName,
            Email = member.Email,
            PhoneNumber = member.GetPhoneNumber() ?? string.Empty,
            PhoneExtension = member.GetPhoneExtension(),
            Apartment = member.Apartment,
            Street = member.Street ?? string.Empty,
            City = member.City ?? string.Empty,
            ZipCode = member.ZipCode ?? string.Empty,
            Roles = roles
        };
    }
}
