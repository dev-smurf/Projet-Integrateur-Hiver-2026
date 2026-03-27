using Application.Interfaces.Services.Users;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Members.Equipe;

public class MyEquipeResponse
{
    public string? Id { get; set; }
    public string? NameFr { get; set; }
    public string? NameEn { get; set; }
    public List<EquipeMemberResponse> Members { get; set; } = [];
    public List<EquipeModuleResponse> Modules { get; set; } = [];
}

public class EquipeMemberResponse
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
}

public class EquipeModuleResponse
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

public class GetMyEquipeEndpoint : EndpointWithoutRequest<MyEquipeResponse>
{
    private readonly IEquipeRepository _equipeRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IAuthenticatedUserService _authService;

    public GetMyEquipeEndpoint(
        IEquipeRepository equipeRepository,
        IMemberRepository memberRepository,
        IAuthenticatedUserService authService)
    {
        _equipeRepository = equipeRepository;
        _memberRepository = memberRepository;
        _authService = authService;
    }

    public override void Configure()
    {
        Get("members/me/equipe");
        Roles(Domain.Constants.User.Roles.MEMBER);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var user = _authService.GetAuthenticatedUser();
        if (user == null)
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var equipe = await _equipeRepository.FindByUserId(user.Id);
        if (equipe == null)
        {
            await Send.OkAsync(new MyEquipeResponse(), ct);
            return;
        }

        // Récupérer les infos des membres de l'équipe
        var members = new List<EquipeMemberResponse>();
        foreach (var membre in equipe.Membres)
        {
            var member = _memberRepository.FindByUserId(membre.Id);
            if (member != null)
            {
                members.Add(new EquipeMemberResponse
                {
                    Id = member.Id.ToString(),
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    Email = member.User.Email ?? ""
                });
            }
        }

        // Récupérer les modules du membre connecté
        var currentMember = _memberRepository.FindByUserId(user.Id);
        var modules = new List<EquipeModuleResponse>();
        if (currentMember != null)
        {
            var memberModules = await _memberRepository.GetMemberModules(currentMember.Id);
            modules = memberModules.Select(mm => new EquipeModuleResponse
            {
                ModuleId = mm.ModuleId.ToString(),
                NameFr = mm.Module?.NameFr,
                NameEn = mm.Module?.NameEn,
                SujetFr = mm.Module?.SujetFr,
                SujetEn = mm.Module?.SujetEn,
                CardImageUrl = mm.Module?.CardImageUrl,
                ProgressPercent = mm.ProgressPercent,
                IsCompleted = mm.IsCompleted
            }).ToList();
        }

        await Send.OkAsync(new MyEquipeResponse
        {
            Id = equipe.Id.ToString(),
            NameFr = equipe.NameFr,
            NameEn = equipe.NameEn,
            Members = members,
            Modules = modules
        }, ct);
    }
}