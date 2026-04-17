using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Equipe;

public class GetEquipeStatsEndpoint : EndpointWithoutRequest<EquipeStatsResponse>
{
    private readonly IMemberEquipeRepository _memberEquipeRepository;
    private readonly IMemberModuleRepository _memberModuleRepository;

    public GetEquipeStatsEndpoint(
        IMemberEquipeRepository memberEquipeRepository,
        IMemberModuleRepository memberModuleRepository)
    {
        _memberEquipeRepository = memberEquipeRepository;
        _memberModuleRepository = memberModuleRepository;
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

        var members = await _memberEquipeRepository.GetByEquipeIdAsync(equipeId);
        var memberCount = members.Count();

        var totalProgress = 0;
        var totalModules = 0;

        foreach (var me in members)
        {
            var modules = await _memberModuleRepository.GetByMemberIdAsync(me.MemberId);
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