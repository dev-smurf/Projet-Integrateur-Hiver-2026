using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Dtos;

namespace Web.Features.Admins.Notes.GetAllNotes;

public class GetAllNotesEndpoint : EndpointWithoutRequest<IEnumerable<NoteDto>>
{
    private readonly IMemberNoteRepository _noteRepository;

    public GetAllNotesEndpoint(IMemberNoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    public override void Configure()
    {
        DontCatchExceptions();
        Get("admin/notes");

        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var notes = await _noteRepository.GetAllAsync(ct);
        var dtos = notes.Select(n => new NoteDto
        {
            Id = n.Id,
            MemberId = n.MemberId,
            MemberName = n.Member.FullName,
            CreatedByAdminId = n.CreatedByAdminId,
            CreatedByAdminName = n.CreatedByAdmin.FullName,
            Content = n.Content,
            IsPrivate = n.IsPrivate,
            Created = n.Created.ToDateTimeUtc()
        });

        Response = dtos;
    }
}
