using Domain.Common;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Equipe;

public class RemoveMemberFromEquipeEndpoint : Endpoint<RemoveMemberFromEquipeRequest, SucceededOrNotResponse>
{
    private readonly IEquipeRepository _equipeRepository;
    private readonly IMemberRepository _memberRepository;

    public RemoveMemberFromEquipeEndpoint(IEquipeRepository equipeRepository, IMemberRepository memberRepository)
    {
        _equipeRepository = equipeRepository;
        _memberRepository = memberRepository;
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

        var equipe = await _equipeRepository.FindByIdWithMembers(equipeId);
        if (equipe == null)
        {
            await Send.OkAsync(new SucceededOrNotResponse(false,
                new Error("NotFound", "Equipe introuvable.")), ct);
            return;
        }

        var member = _memberRepository.FindById(memberId);
        var user = equipe.Membres.FirstOrDefault(m => m.Id == member.User.Id);

        if (user == null)
        {
            await Send.OkAsync(new SucceededOrNotResponse(false,
                new Error("NotFound", "Assignation introuvable.")), ct);
            return;
        }

        equipe.Membres.Remove(user);
        await _equipeRepository.UpdateEquipe(equipe);
        await Send.OkAsync(new SucceededOrNotResponse(true), cancellation: ct);
    }
}

public class RemoveMemberFromEquipeRequest
{
    public string EquipeId { get; set; } = null!;
    public string MemberId { get; set; } = null!;
}
