using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Domain.Entities;
using Domain.Repositories;
using Domain.Common;

public class CreateEquipeEndpoint : Endpoint<CreateEquipeRequest, SucceededOrNotResponse>
{
    private readonly IEquipeRepository _equipeService;

    public CreateEquipeEndpoint(IEquipeRepository equipeService)
    {
        _equipeService = equipeService;
    }

    public override void Configure()
    {
        AllowFileUploads();
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
            NameEn = req.NameEn
        };

        newEquipe.SanitazeForSaving();

        await _equipeService.CreateEquipe(newEquipe);
        await Send.OkAsync(new SucceededOrNotResponse(true));
    }
}

