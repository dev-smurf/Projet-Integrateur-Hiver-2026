using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Domain.Entities;
using Domain.Repositories;
using Domain.Common;

public class CreateEquipeEndpoint : Endpoint<CreateEquipeRequest, SucceededOrNotResponse>
{
    private readonly IEquipeRepository _equipeRepository;
    private readonly IMemberEquipeRepository _memberEquipeRepository;

    public CreateEquipeEndpoint(IEquipeRepository equipeRepository, IMemberEquipeRepository memberEquipeRepository)
    {
        _equipeRepository = equipeRepository;
        _memberEquipeRepository = memberEquipeRepository;
    }

    public override void Configure()
    {
        Post("equipes");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        DontCatchExceptions();
    }

    public override async Task HandleAsync(CreateEquipeRequest req, CancellationToken ct)
    {
        req.Sanitize();

        var newEquipe = new Equipe(req.NameFr!, req.NameEn ?? string.Empty, req.ParentEquipeId?.ToString());
        newEquipe.SanitazeForSaving();

        await _equipeRepository.CreateEquipe(newEquipe);

        if (req.MemberIds?.Any() == true)
        {
            foreach (var memberId in req.MemberIds.Where(id => id != Guid.Empty).Distinct())
            {
                await _memberEquipeRepository.AssignAsync(new MemberEquipe(memberId, newEquipe.Id));
            }
        }

        await Send.OkAsync(new SucceededOrNotResponse(true));
    }
}