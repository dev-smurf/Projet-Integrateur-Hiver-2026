using Application.Interfaces.Services.Equipe.Dto;
using Application.Interfaces.Services.Members;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Members.Equipe;

public class GetMyEquipesEndpoint : EndpointWithoutRequest<List<EquipeDto>>
{
    private readonly IAuthenticatedMemberService _authenticatedMemberService;
    private readonly IEquipeRepository _equipeRepository;

    public GetMyEquipesEndpoint(
        IAuthenticatedMemberService authenticatedMemberService,
        IEquipeRepository equipeRepository)
    {
        _authenticatedMemberService = authenticatedMemberService;
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

        var equipeIds = await _equipeRepository.GetEquipeIdsForUser(member.User.Id);
        var equipes = await _equipeRepository.GetByIds(equipeIds);

        var dtos = equipes
            .DistinctBy(e => e.Id)
            .Select(e => new EquipeDto
            {
                Id = e.Id.ToString(),
                NameFr = e.NameFr,
                NameEn = e.NameEn,
                ParentEquipeId = e.ParentEquipeId?.ToString(),
                ParentEquipeNameFr = e.ParentEquipe?.NameFr,
                ParentEquipeNameEn = e.ParentEquipe?.NameEn
            })
            .ToList();

        await Send.OkAsync(dtos, cancellation: ct);
    }
}
