using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Equipe;

public class AssignModuleToEquipeEndpoint : Endpoint<AssignModuleToEquipeRequest, SucceededOrNotResponse>
{
    private readonly IEquipeRepository _equipeRepository;
    private readonly IMemberModuleRepository _memberModuleRepository;
    private readonly IMemberRepository _memberRepository;

    public AssignModuleToEquipeEndpoint(
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
        Post("equipes/{equipeId}/assign-module");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        DontCatchExceptions();
    }

    public override async Task HandleAsync(AssignModuleToEquipeRequest req, CancellationToken ct)
    {
        if (!Guid.TryParse(Route<string>("equipeId"), out var equipeId)
            || !Guid.TryParse(req.ModuleId, out var moduleId))
        {
            await Send.OkAsync(new SucceededOrNotResponse(false,
                new Error("InvalidId", "ID invalide.")), ct);
            return;
        }

        var equipe = await _equipeRepository.FindByIdWithMembers(equipeId);
        if (equipe == null)
        {
            await Send.OkAsync(new SucceededOrNotResponse(false,
                new Error("TeamNotFound", "Team not found.")), ct);
            return;
        }

        foreach (var user in equipe.Membres)
        {
            var member = _memberRepository.FindByUserId(user.Id);
            if (member == null)
                continue;

            if (!await _memberModuleRepository.IsAssignedAsync(member.Id, moduleId))
                await _memberModuleRepository.AssignAsync(new MemberModule(member.Id, moduleId));
        }

        await Send.OkAsync(new SucceededOrNotResponse(true));
    }
}

public class AssignModuleToEquipeRequest
{
    public string ModuleId { get; set; } = null!;
}
