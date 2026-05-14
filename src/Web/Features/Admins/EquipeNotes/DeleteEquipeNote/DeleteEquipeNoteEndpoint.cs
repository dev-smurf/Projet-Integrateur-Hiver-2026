using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.EquipeNotes.DeleteEquipeNote;

public class DeleteEquipeNoteRequest
{
    public Guid Id { get; set; }
}

public class DeleteEquipeNoteEndpoint : Endpoint<DeleteEquipeNoteRequest>
{
    private readonly IEquipeNoteRepository _noteRepository;

    public DeleteEquipeNoteEndpoint(IEquipeNoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    public override void Configure()
    {
        Delete("admin/equipe-notes/{id}");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(DeleteEquipeNoteRequest req, CancellationToken ct)
    {
        var note = await _noteRepository.GetByIdAsync(req.Id, ct);
        if (note == null)
        {
            HttpContext.Response.StatusCode = 404;
            return;
        }

        await _noteRepository.DeleteAsync(note, ct);
        HttpContext.Response.StatusCode = 204;
    }
}
