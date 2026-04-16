using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Equipe;

public class AssignModuleToEquipeEndpoint : Endpoint<AssignModuleToEquipeRequest, SucceededOrNotResponse>
{
    private readonly IMemberEquipeRepository _memberEquipeRepository;
    private readonly IMemberModuleRepository _memberModuleRepository;

    public AssignModuleToEquipeEndpoint(
        IMemberEquipeRepository memberEquipeRepository,
        IMemberModuleRepository memberModuleRepository)
    {
        _memberEquipeRepository = memberEquipeRepository;
        _memberModuleRepository = memberModuleRepository;
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

        var members = await _memberEquipeRepository.GetByEquipeIdAsync(equipeId);

        foreach (var me in members)
        {
            if (!await _memberModuleRepository.IsAssignedAsync(me.MemberId, moduleId))
            {
                await _memberModuleRepository.AssignAsync(new MemberModule(me.MemberId, moduleId));
            }
        }

        await Send.OkAsync(new SucceededOrNotResponse(true));
    }
}

public class AssignModuleToEquipeRequest
{
    public string ModuleId { get; set; } = null!;
}