using Application.Interfaces.Services.Admins;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Dtos;

namespace Web.Features.Admins.Notes.UpdateNote;

public class UpdateNoteEndpoint : Endpoint<UpdateNoteRequest, NoteDto>
{
    private readonly IMemberNoteRepository _noteRepository;
    private readonly IAuthenticatedAdminService _adminService;

    public UpdateNoteEndpoint(
        IMemberNoteRepository noteRepository,
        IAuthenticatedAdminService adminService)
    {
        _noteRepository = noteRepository;
        _adminService = adminService;
    }

    public override void Configure()
    {
        DontCatchExceptions();
        Put("admin/notes/{id}");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(UpdateNoteRequest req, CancellationToken ct)
    {
        var admin = _adminService.GetAuthenticatedAdmin();
        if (admin == null)
        {
            HttpContext.Response.StatusCode = 403;
            return;
        }

        var note = await _noteRepository.GetByIdAsync(req.Id, ct);
        if (note == null)
        {
            HttpContext.Response.StatusCode = 404;
            return;
        }

        note.UpdateContent(req.Content);
        note.SetPrivacy(req.IsPrivate);

        await _noteRepository.UpdateAsync(note, ct);

        // Fetch Member details to populate Dto fully if needed. The repository GetByIdAsync might Include them.
        var dto = new NoteDto
        {
            Id = note.Id,
            MemberId = note.MemberId,
            MemberName = note.Member?.FullName ?? "",
            CreatedByAdminId = note.CreatedByAdminId,
            CreatedByAdminName = note.CreatedByAdmin?.FullName ?? "",
            Content = note.Content,
            IsPrivate = note.IsPrivate,
            Created = note.Created.ToDateTimeUtc()
        };

        Response = dto;
    }
}
