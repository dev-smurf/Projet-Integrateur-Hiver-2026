using Domain.Common;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Equipe;

public class UnassignModuleFromEquipeEndpoint : EndpointWithoutRequest<SucceededOrNotResponse>
{
    private readonly IEquipeRepository _equipeRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IMemberModuleRepository _memberModuleRepository;

    public UnassignModuleFromEquipeEndpoint(
        IEquipeRepository equipeRepository,
        IMemberRepository memberRepository,
        IMemberModuleRepository memberModuleRepository)
    {
        _equipeRepository = equipeRepository;
        _memberRepository = memberRepository;
        _memberModuleRepository = memberModuleRepository;
    }

    public override void Configure()
    {
        Delete("equipes/{equipeId}/modules/{moduleId}");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        DontCatchExceptions();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        if (!Guid.TryParse(Route<string>("equipeId"), out var equipeId)
            || !Guid.TryParse(Route<string>("moduleId"), out var moduleId))
        {
            await Send.OkAsync(new SucceededOrNotResponse(false,
                new Error("InvalidId", "ID invalide.")), ct);
            return;
        }

        var equipe = await _equipeRepository.FindByIdWithMembers(equipeId);
        if (equipe == null)
        {
            await Send.OkAsync(new SucceededOrNotResponse(false,
                new Error("TeamNotFound", "Équipe introuvable.")), ct);
            return;
        }

        foreach (var user in equipe.Membres)
        {
            var member = _memberRepository.FindByUserId(user.Id);
            if (member == null) continue;

            var assignment = await _memberModuleRepository.GetByMemberAndModuleAsync(member.Id, moduleId);
            if (assignment != null)
                await _memberModuleRepository.UnassignAsync(assignment);
        }

        await Send.OkAsync(new SucceededOrNotResponse(true), ct);
    }
}