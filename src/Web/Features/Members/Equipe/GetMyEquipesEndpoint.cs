using Application.Interfaces.Services.Equipe.Dto;
using Application.Interfaces.Services.Members;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Members.Equipe;

public class GetMyEquipesEndpoint : EndpointWithoutRequest<List<EquipeDto>>
{
    private readonly IAuthenticatedMemberService _authenticatedMemberService;
    private readonly IMemberEquipeRepository _memberEquipeRepository;
    private readonly IEquipeRepository _equipeRepository;

    public GetMyEquipesEndpoint(
        IAuthenticatedMemberService authenticatedMemberService,
        IMemberEquipeRepository memberEquipeRepository,
        IEquipeRepository equipeRepository)
    {
        _authenticatedMemberService = authenticatedMemberService;
        _memberEquipeRepository = memberEquipeRepository;
        _equipeRepository = equipeRepository;
    }

    public override void Configure()
    {
        Get("members/me/equipes");
        Roles(Domain.Constants.User.Roles.MEMBER);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        DontCatchExceptions();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var member = _authenticatedMemberService.GetAuthenticatedMember();
        var equipes = await GetMemberEquipesAsync(member);

        await Send.OkAsync(equipes, cancellation: ct);
    }

    private async Task<List<EquipeDto>> GetMemberEquipesAsync(Domain.Entities.Member member)
    {
        try
        {
            var assignments = await _memberEquipeRepository.GetByMemberIdAsync(member.Id);
            var equipes = assignments
                .Select(a => a.Equipe)
                .Where(e => e != null)
                .DistinctBy(e => e.Id)
                .Select(MapEquipe)
                .ToList();

            if (equipes.Count > 0)
            {
                return equipes;
            }
        }
        catch
        {
        }

        var equipeIds = await _equipeRepository.GetEquipeIdsForUser(member.User.Id);
        var fallbackEquipes = await _equipeRepository.GetByIds(equipeIds);

        return fallbackEquipes
            .DistinctBy(e => e.Id)
            .Select(MapEquipe)
            .ToList();
    }

    private static EquipeDto MapEquipe(Domain.Entities.Equipe e)
    {
        return new EquipeDto
        {
            Id = e.Id.ToString(),
            NameFr = e.NameFr,
            NameEn = e.NameEn,
            ParentEquipeId = e.ParentEquipeId?.ToString(),
            ParentEquipeNameFr = e.ParentEquipe?.NameFr,
            ParentEquipeNameEn = e.ParentEquipe?.NameEn
        };
    }
}
