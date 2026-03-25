using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Dtos;
using Domain.Entities;

namespace Web.Features.Admins.Members.GetMember;

public class GetMemberEndpoint : Endpoint<GetMemberRequest, MemberDto>
{
    private readonly IMemberRepository _memberRepository;

    public GetMemberEndpoint(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public override void Configure()
    {
        DontCatchExceptions();
        Get("members/{id}");

        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(GetMemberRequest req, CancellationToken ct)
    {
        var member = _memberRepository.FindById(req.Id);
        var response = MapMember(member);
        await Send.OkAsync(response, cancellation: ct);
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
