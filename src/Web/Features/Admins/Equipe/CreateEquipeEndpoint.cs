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

        var newEquipe = new Equipe
        {
            NameFr = req.NameFr,
            NameEn = req.NameEn ?? string.Empty
        };

        newEquipe.SanitazeForSaving();

        await _equipeService.CreateEquipe(newEquipe);

        // Assign the selected members (if any).
        var userIds = (req.MemberUserIds ?? new List<string>())
            .Select(s => Guid.TryParse(s, out var g) ? g : Guid.Empty)
            .Where(g => g != Guid.Empty)
            .ToList();
        if (userIds.Count > 0)
        {
            await _equipeService.SetEquipeMembers(newEquipe.Id, userIds);
        }

        await Send.OkAsync(new SucceededOrNotResponse(true));
    }
}

