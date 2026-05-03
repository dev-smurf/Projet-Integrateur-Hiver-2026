using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Equipe;

public class GetEquipeMembersEndpoint : Endpoint<GetEquipeMembersRequest, GetEquipeMembersResponse>
{
    private readonly IEquipeRepository _equipeRepository;
    private readonly IMemberRepository _memberRepository;

    public GetEquipeMembersEndpoint(IEquipeRepository equipeRepository, IMemberRepository memberRepository)
    {
        _equipeRepository = equipeRepository;
        _memberRepository = memberRepository;
    }

    public override void Configure()
    {
        Get("equipes/{equipeId}/members");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        DontCatchExceptions();
    }

    public override async Task HandleAsync(GetEquipeMembersRequest req, CancellationToken ct)
    {
        if (!Guid.TryParse(req.EquipeId, out var equipeId))
        {
            HttpContext.Response.StatusCode = 400;
            return;
        }

        var equipe = await _equipeRepository.FindByIdWithMembers(equipeId);
        if (equipe == null)
        {
            HttpContext.Response.StatusCode = 404;
            return;
        }

        var members = equipe.MemberEquipes
            .Where(me => me.Member != null && me.Member.User != null)
            .Select(me => new GetEquipeMemberDto
            {
                MemberId = me.MemberId.ToString(),
                UserId = me.Member.User.Id.ToString(),
                FirstName = me.Member.FirstName,
                LastName = me.Member.LastName,
                Email = me.Member.Email
            })
            .ToList();

        if (members.Count == 0 && equipe.Membres.Count > 0)
        {
            members = equipe.Membres
                .Select(user => _memberRepository.FindByUserId(user.Id))
                .Where(member => member != null)
                .Select(member => new GetEquipeMemberDto
                {
                    MemberId = member!.Id.ToString(),
                    UserId = member.User.Id.ToString(),
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    Email = member.Email
                })
                .DistinctBy(member => member.MemberId)
                .ToList();
        }

        var response = new GetEquipeMembersResponse
        {
            EquipeId = req.EquipeId,
            Members = members
        };

        await Send.OkAsync(response, cancellation: ct);
    }
}

public class GetEquipeMembersRequest
{
    public string EquipeId { get; set; } = null!;
}

public class GetEquipeMembersResponse
{
    public string EquipeId { get; set; } = null!;
    public List<GetEquipeMemberDto> Members { get; set; } = new();
}

public class GetEquipeMemberDto
{
    public string MemberId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
}
