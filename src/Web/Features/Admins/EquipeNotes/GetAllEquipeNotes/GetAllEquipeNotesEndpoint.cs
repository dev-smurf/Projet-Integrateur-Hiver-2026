using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Collections.Generic;
using System.Linq;
using Web.Dtos;

namespace Web.Features.Admins.EquipeNotes.GetAllEquipeNotes;

public class GetAllEquipeNotesEndpoint : EndpointWithoutRequest<IEnumerable<EquipeNoteDto>>
{
    private readonly IEquipeNoteRepository _noteRepository;

    public GetAllEquipeNotesEndpoint(IEquipeNoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    public override void Configure()
    {
        Get("admin/equipe-notes");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var notes = await _noteRepository.GetAllAsync(ct);

        Response = notes.Select(n => new EquipeNoteDto
        {
            Id = n.Id,
            EquipeId = n.EquipeId,
            EquipeName = n.Equipe?.NameFr ?? string.Empty,
            CreatedByAdminId = n.CreatedByAdminId,
            CreatedByAdminName = n.CreatedByAdmin?.FullName ?? string.Empty,
            Content = n.Content,
            IsPrivate = n.IsPrivate,
            Created = n.Created.ToDateTimeUtc()
        });
    }
}
