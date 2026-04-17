using Domain.Common;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Equipe;

public class RemoveMemberFromEquipeEndpoint : Endpoint<RemoveMemberFromEquipeRequest, SucceededOrNotResponse>
{
    private readonly IMemberEquipeRepository _memberEquipeRepository;

    public RemoveMemberFromEquipeEndpoint(IMemberEquipeRepository memberEquipeRepository)
    {
        _memberEquipeRepository = memberEquipeRepository;
    }

    public override void Configure()
    {
        Delete("equipes/{equipeId}/members/{memberId}");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        DontCatchExceptions();
    }

    public override async Task HandleAsync(RemoveMemberFromEquipeRequest req, CancellationToken ct)
    {
        if (!Guid.TryParse(req.EquipeId, out var equipeId) || !Guid.TryParse(req.MemberId, out var memberId))
        {
            await Send.OkAsync(new SucceededOrNotResponse(false,
                new Error("InvalidId", "ID invalide.")), ct);
            return;
        }

        var memberEquipe = await _memberEquipeRepository.GetByMemberAndEquipeAsync(memberId, equipeId);

        if (memberEquipe == null)
        {
            await Send.OkAsync(new SucceededOrNotResponse(false,
                new Error("NotFound", "Assignation introuvable.")), ct);
            return;
        }

        await _memberEquipeRepository.UnassignAsync(memberEquipe);
        await Send.OkAsync(new SucceededOrNotResponse(true), cancellation: ct);
    }
}

public class RemoveMemberFromEquipeRequest
{
    public string EquipeId { get; set; } = null!;
    public string MemberId { get; set; } = null!;
}