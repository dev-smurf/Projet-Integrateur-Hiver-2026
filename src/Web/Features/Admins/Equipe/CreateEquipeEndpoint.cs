using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

public class CreateEquipeEndpoint : Endpoint<CreateEquipeRequest, SucceededOrNotResponse>
{
    private readonly IEquipeRepository _equipeRepository;
    private readonly IMemberRepository _memberRepository;

    public CreateEquipeEndpoint(IEquipeRepository equipeRepository, IMemberRepository memberRepository)
    {
        _equipeRepository = equipeRepository;
        _memberRepository = memberRepository;
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

        await _equipeRepository.CreateEquipe(newEquipe);

        var memberUserIds = (req.MemberIds ?? [])
            .Where(id => id != Guid.Empty)
            .Distinct()
            .Select(id => _memberRepository.FindById(id).User.Id)
            .Distinct()
            .ToList();

        if (memberUserIds.Count > 0)
            await _equipeRepository.SetEquipeMembers(newEquipe.Id, memberUserIds);

        await Send.OkAsync(new SucceededOrNotResponse(true));
    }
}
