using Domain.Common;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Dtos;
using Web.Features.Common;
using Domain.Entities;
using Domain.Extensions;

namespace Web.Features.Admins.Members.GetAllMembers;

public class SearchMembersEndpoint : Endpoint<PaginateRequest, PaginatedList<MemberDto>>
{
    private readonly IMemberRepository _memberRepository;

    public SearchMembersEndpoint(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public override void Configure()
    {
        DontCatchExceptions();
        Get("members");

        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(PaginateRequest req, CancellationToken ct)
    {
        var paginatedList = _memberRepository.GetAllPaginated(req.PageIndex, req.PageSize, req.SearchValue);
        var membersDto = paginatedList.Items.Select(MapMember).ToList();
        await Send.OkAsync(new PaginatedList<MemberDto>(membersDto, paginatedList.TotalItems), cancellation: ct);
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
