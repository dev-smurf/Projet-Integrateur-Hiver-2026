using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Dtos;

namespace Web.Features.Admins.Members.GetMember;

public class GetMemberEndpoint : Endpoint<GetMemberRequest, MemberDto>
{
    private readonly IMemberRepository _memberRepository;
    private readonly IMemberEquipeRepository _memberEquipeRepository;
    private readonly IEquipeRepository _equipeRepository;

    public GetMemberEndpoint(
        IMemberRepository memberRepository,
        IMemberEquipeRepository memberEquipeRepository,
        IEquipeRepository equipeRepository)
    {
        _memberRepository = memberRepository;
        _memberEquipeRepository = memberEquipeRepository;
        _equipeRepository = equipeRepository;
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
        var response = await MapMember(member);
        await Send.OkAsync(response, cancellation: ct);
    }

    private async Task<MemberDto> MapMember(Member member)
    {
        var roles = member.User.UserRoles.Select(r => r.Role.Name ?? string.Empty).Where(r => !string.IsNullOrWhiteSpace(r)).ToList();
        var equipeIds = await GetEquipeIdsAsync(member);
        return new MemberDto
        {
            Id = member.Id,
            UserId = member.User.Id,
            Created = member.Created.ToDateTimeUtc(),
            Active = member.Active,
            AccountActivated = !string.IsNullOrWhiteSpace(member.User.PasswordHash),
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
            AdminNotes = member.AdminNotes,
            AdminNotesVisibleToMember = member.AdminNotesVisibleToMember,
            Roles = roles,
            EquipeIds = equipeIds
        };
    }

    private async Task<List<Guid>> GetEquipeIdsAsync(Member member)
    {
        try
        {
            return await _memberEquipeRepository.GetEquipeIdsForMemberAsync(member.Id);
        }
        catch
        {
            return await _equipeRepository.GetEquipeIdsForUser(member.User.Id);
        }
    }
}
