using Application.Interfaces.Services.Admins;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Notes.DeleteNote;

public class DeleteNoteEndpoint : EndpointWithoutRequest
{
    private readonly IMemberNoteRepository _noteRepository;
    private readonly IAuthenticatedAdminService _adminService;

    public DeleteNoteEndpoint(
        IMemberNoteRepository noteRepository,
        IAuthenticatedAdminService adminService)
    {
        _noteRepository = noteRepository;
        _adminService = adminService;
    }

    public override void Configure()
    {
        DontCatchExceptions();
        Delete("admin/notes/{id}");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var admin = _adminService.GetAuthenticatedAdmin();
        if (admin == null)
        {
            HttpContext.Response.StatusCode = 403;
            return;
        }

        var idStr = Route<string>("id");
        if (!Guid.TryParse(idStr, out var id))
        {
            HttpContext.Response.StatusCode = 400;
            return;
        }

        var note = await _noteRepository.GetByIdAsync(id, ct);
        if (note == null)
        {
            HttpContext.Response.StatusCode = 404;
            return;
        }

        await _noteRepository.DeleteAsync(note, ct);

        HttpContext.Response.StatusCode = 204;
    }
}
