using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Equipe;

public class GetEquipeModulesResponse
{
    public string EquipeId { get; set; } = null!;
    public List<EquipeModuleDto> Modules { get; set; } = new();
}

public class EquipeModuleDto
{
    public string ModuleId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? CardImageUrl { get; set; }
}

public class GetEquipeModulesEndpoint : EndpointWithoutRequest<GetEquipeModulesResponse>
{
    private readonly IEquipeRepository _equipeRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IMemberModuleRepository _memberModuleRepository;
    private readonly IModuleRepository _moduleRepository;

    public GetEquipeModulesEndpoint(
        IEquipeRepository equipeRepository,
        IMemberRepository memberRepository,
        IMemberModuleRepository memberModuleRepository,
        IModuleRepository moduleRepository)
    {
        _equipeRepository = equipeRepository;
        _memberRepository = memberRepository;
        _memberModuleRepository = memberModuleRepository;
        _moduleRepository = moduleRepository;
    }

    public override void Configure()
    {
        Get("equipes/{equipeId}/modules");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        DontCatchExceptions();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        if (!Guid.TryParse(Route<string>("equipeId"), out var equipeId))
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var equipe = await _equipeRepository.FindByIdWithMembers(equipeId);
        if (equipe == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var moduleIdSets = new List<HashSet<Guid>>();
        foreach (var user in equipe.Membres)
        {
            var member = _memberRepository.FindByUserId(user.Id);
            if (member == null) continue;

            var memberModules = await _memberModuleRepository.GetByMemberIdAsync(member.Id);
            moduleIdSets.Add(memberModules.Select(mm => mm.ModuleId).ToHashSet());
        }

        var commonModuleIds = moduleIdSets.Count == 0
            ? new HashSet<Guid>()
            : moduleIdSets.Aggregate((a, b) => a.Intersect(b).ToHashSet());

        var modules = new List<EquipeModuleDto>();
        foreach (var moduleId in commonModuleIds)
        {
            var module = await _moduleRepository.GetByIdAsync(moduleId);
            if (module != null)
                modules.Add(new EquipeModuleDto
                {
                    ModuleId = module.Id.ToString(),
                    Name = module.Name,
                    CardImageUrl = module.CardImageUrl
                });
        }

        await Send.OkAsync(new GetEquipeModulesResponse
        {
            EquipeId = equipeId.ToString(),
            Modules = modules
        }, ct);
    }
}