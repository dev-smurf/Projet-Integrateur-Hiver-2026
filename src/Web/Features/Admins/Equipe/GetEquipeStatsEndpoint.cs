using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Equipe;

public class GetEquipeStatsEndpoint : EndpointWithoutRequest<EquipeStatsResponse>
{
    private readonly IEquipeRepository _equipeRepository;
    private readonly IMemberModuleRepository _memberModuleRepository;
    private readonly IMemberRepository _memberRepository;

    public GetEquipeStatsEndpoint(
        IEquipeRepository equipeRepository,
        IMemberModuleRepository memberModuleRepository,
        IMemberRepository memberRepository)
    {
        _equipeRepository = equipeRepository;
        _memberModuleRepository = memberModuleRepository;
        _memberRepository = memberRepository;
    }

    public override void Configure()
    {
        Get("equipes/{equipeId}/stats");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        DontCatchExceptions();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        if (!Guid.TryParse(Route<string>("equipeId"), out var equipeId))
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

        var memberCount = equipe.Membres.Count;
        var totalProgress = 0;
        var totalModules = 0;

        foreach (var user in equipe.Membres)
        {
            var member = _memberRepository.FindByUserId(user.Id);
            if (member == null)
                continue;

            var modules = await _memberModuleRepository.GetByMemberIdAsync(member.Id);
            foreach (var mm in modules)
            {
                totalProgress += mm.ProgressPercent;
                totalModules++;
            }
        }

        Response = new EquipeStatsResponse
        {
            MemberCount = memberCount,
            TotalModulesAssigned = totalModules,
            AverageProgress = totalModules > 0 ? totalProgress / totalModules : 0
        };
    }
}

public class EquipeStatsResponse
{
    public int MemberCount { get; set; }
    public int TotalModulesAssigned { get; set; }
    public int AverageProgress { get; set; }
}
