using Application.Interfaces.Services.Members;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Members.Equipe;

public class GetMyEquipeEndpoint : EndpointWithoutRequest<GetMyEquipeResponse?>
{
    private readonly IAuthenticatedMemberService _authenticatedMemberService;
    private readonly IMemberEquipeRepository _memberEquipeRepository;
    private readonly IEquipeRepository _equipeRepository;
    private readonly IMemberRepository _memberRepository;

    public GetMyEquipeEndpoint(
        IAuthenticatedMemberService authenticatedMemberService,
        IMemberEquipeRepository memberEquipeRepository,
        IEquipeRepository equipeRepository,
        IMemberRepository memberRepository)
    {
        _authenticatedMemberService = authenticatedMemberService;
        _memberEquipeRepository = memberEquipeRepository;
        _equipeRepository = equipeRepository;
        _memberRepository = memberRepository;
    }

    public override void Configure()
    {
        Get("members/me/equipe");
        Roles(Domain.Constants.User.Roles.MEMBER);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        DontCatchExceptions();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var member = _authenticatedMemberService.GetAuthenticatedMember();

        var equipeIds = await _memberEquipeRepository.GetEquipeIdsForMemberAsync(member.Id);

        if (equipeIds.Count == 0)
        {
            await Send.OkAsync(null, cancellation: ct);
            return;
        }

        var equipe = await _equipeRepository.FindByIdWithMembers(equipeIds.First());

        if (equipe == null)
        {
            await Send.OkAsync(null, cancellation: ct);
            return;
        }

        var memberModules = await _memberRepository.GetMemberModules(member.Id);

        var response = new GetMyEquipeResponse
        {
            Id = equipe.Id.ToString(),
            NameFr = equipe.NameFr,
            NameEn = equipe.NameEn,
            Members = equipe.MemberEquipes.Select(me => new GetMyEquipeMemberDto
            {
                Id = me.MemberId.ToString(),
                FirstName = me.Member.FirstName,
                LastName = me.Member.LastName,
                Email = me.Member.Email
            }).ToList(),
            Modules = memberModules.Select(mm => new GetMyEquipeModuleDto
            {
                ModuleId = mm.ModuleId.ToString(),
                NameFr = mm.Module.Name,
                NameEn = mm.Module.Name,
                CardImageUrl = mm.Module.CardImageUrl,
                ProgressPercent = mm.ProgressPercent,
                IsCompleted = mm.IsCompleted
            }).ToList()
        };

        await Send.OkAsync(response, cancellation: ct);
    }
}

public class GetMyEquipeResponse
{
    public string Id { get; set; } = null!;
    public string? NameFr { get; set; }
    public string? NameEn { get; set; }
    public List<GetMyEquipeMemberDto> Members { get; set; } = new();
    public List<GetMyEquipeModuleDto> Modules { get; set; } = new();
}

public class GetMyEquipeMemberDto
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
}

public class GetMyEquipeModuleDto
{
    public string ModuleId { get; set; } = null!;
    public string? NameFr { get; set; }
    public string? NameEn { get; set; }
    public string? SujetFr { get; set; }
    public string? SujetEn { get; set; }
    public string? CardImageUrl { get; set; }
    public int ProgressPercent { get; set; }
    public bool IsCompleted { get; set; }
}