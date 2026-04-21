using Domain.Common;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Equipe;

public class AssignMembersToEquipeEndpoint : Endpoint<AssignMembersToEquipeRequest, SucceededOrNotResponse>
{
    private readonly IEquipeRepository _equipeRepository;
    private readonly IMemberRepository _memberRepository;

    public AssignMembersToEquipeEndpoint(IEquipeRepository equipeRepository, IMemberRepository memberRepository)
    {
        _equipeRepository = equipeRepository;
        _memberRepository = memberRepository;
    }

    public override void Configure()
    {
        Post("equipes/{equipeId}/assign-members");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        DontCatchExceptions();
    }

    public override async Task HandleAsync(AssignMembersToEquipeRequest req, CancellationToken ct)
    {
        var equipeIdString = Route<string>("equipeId");

        if (!Guid.TryParse(equipeIdString, out var equipeId))
        {
            await Send.OkAsync(new SucceededOrNotResponse(false,
                new Error("TeamNotFound", "Team not found.")), ct);
            return;
        }

        var equipe = await _equipeRepository.FindById(equipeId);

        if (equipe == null)
        {
            await Send.OkAsync(new SucceededOrNotResponse(false,
                new Error("TeamNotFound", "Team not found.")), ct);
            return;
        }

        var memberUserIds = req.MemberIds
            .Where(id => id != Guid.Empty)
            .Distinct()
            .Select(id => _memberRepository.FindById(id).User.Id)
            .Distinct()
            .ToList();

        await _equipeRepository.SetEquipeMembers(equipeId, memberUserIds);
        await Send.OkAsync(new SucceededOrNotResponse(true));
    }
}
