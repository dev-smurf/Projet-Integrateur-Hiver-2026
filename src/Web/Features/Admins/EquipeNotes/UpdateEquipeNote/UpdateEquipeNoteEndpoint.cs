using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Dtos;

namespace Web.Features.Admins.EquipeNotes.UpdateEquipeNote;

public class UpdateEquipeNoteRequest
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsPrivate { get; set; }
}

public class UpdateEquipeNoteEndpoint : Endpoint<UpdateEquipeNoteRequest, EquipeNoteDto>
{
    private readonly IEquipeNoteRepository _noteRepository;

    public UpdateEquipeNoteEndpoint(IEquipeNoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    public override void Configure()
    {
        Put("admin/equipe-notes/{id}");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(UpdateEquipeNoteRequest req, CancellationToken ct)
    {
        var note = await _noteRepository.GetByIdAsync(req.Id, ct);
        if (note == null)
        {
            HttpContext.Response.StatusCode = 404;
            return;
        }

        note.UpdateContent(req.Content);
        note.SetPrivacy(req.IsPrivate);

        await _noteRepository.UpdateAsync(note, ct);

        Response = new EquipeNoteDto
        {
            Id = note.Id,
            EquipeId = note.EquipeId,
            EquipeName = note.Equipe?.NameFr ?? string.Empty,
            CreatedByAdminId = note.CreatedByAdminId,
            CreatedByAdminName = note.CreatedByAdmin?.FullName ?? string.Empty,
            Content = note.Content,
            IsPrivate = note.IsPrivate,
            Created = note.Created.ToDateTimeUtc()
        };
    }
}
