using Domain.Repositories;
using FastEndpoints;
using Application.Interfaces.Services.Equipe.Dto;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Equipe;

public class GetEquipeByIdEndpoint : EndpointWithoutRequest<EquipeDto>
{
    private readonly IEquipeRepository _repository;

    public GetEquipeByIdEndpoint(IEquipeRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/equipe/{id}");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        DontCatchExceptions();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var idString = Route<string>("id");

        if (!Guid.TryParse(idString, out var guidId))
        {
            HttpContext.Response.StatusCode = 400;
            Response = null!;
            return;
        }

        var entity = await _repository.FindByIdWithMembersAndSousEquipes(guidId);

        if (entity is null)
        {
            HttpContext.Response.StatusCode = 404;
            Response = null!;
            return;
        }

        var memberIds = entity.MemberEquipes
            .Where(me => me.Member != null)
            .Select(me => me.MemberId.ToString())
            .Distinct()
            .ToList();

        var memberUserIds = entity.MemberEquipes
            .Where(me => me.Member?.User != null)
            .Select(me => me.Member.User.Id.ToString())
            .Distinct()
            .ToList();

        if (memberUserIds.Count == 0)
        {
            memberUserIds = entity.Membres
                .Select(u => u.Id.ToString())
                .Distinct()
                .ToList();
        }

        Response = new EquipeDto
        {
            Id = entity.Id.ToString(),
            NameFr = entity.NameFr,
            NameEn = entity.NameEn,
            ParentEquipeId = entity.ParentEquipeId?.ToString(),
            MemberIds = memberIds,
            MemberUserIds = memberUserIds,
            SousEquipes = entity.SousEquipes.Select(s => new EquipeDto
            {
                Id = s.Id.ToString(),
                NameFr = s.NameFr,
                NameEn = s.NameEn,
                ParentEquipeId = s.ParentEquipeId?.ToString()
            }).ToList()
        };
    }
}
