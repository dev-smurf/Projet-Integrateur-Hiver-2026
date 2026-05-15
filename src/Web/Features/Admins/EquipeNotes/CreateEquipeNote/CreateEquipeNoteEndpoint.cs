using Application.Interfaces.Services.Admins;
using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Dtos;

namespace Web.Features.Admins.EquipeNotes.CreateEquipeNote;

public class CreateEquipeNoteEndpoint : Endpoint<CreateEquipeNoteRequest, EquipeNoteDto>
{
    private readonly IEquipeNoteRepository _noteRepository;
    private readonly IEquipeRepository _equipeRepository;
    private readonly IAuthenticatedAdminService _adminService;

    public CreateEquipeNoteEndpoint(
        IEquipeNoteRepository noteRepository,
        IEquipeRepository equipeRepository,
        IAuthenticatedAdminService adminService)
    {
        _noteRepository = noteRepository;
        _equipeRepository = equipeRepository;
        _adminService = adminService;
    }

    public override void Configure()
    {
        Post("admin/equipe-notes");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CreateEquipeNoteRequest req, CancellationToken ct)
    {
        var admin = _adminService.GetAuthenticatedAdmin();
        if (admin == null)
        {
            HttpContext.Response.StatusCode = 403;
            return;
        }

        var equipe = await _equipeRepository.FindById(req.EquipeId);
        if (equipe == null)
        {
            HttpContext.Response.StatusCode = 404;
            return;
        }

        var note = new EquipeNote(equipe.Id, admin.Id, req.Content, req.IsPrivate);
        await _noteRepository.AddAsync(note, ct);

        Response = new EquipeNoteDto
        {
            Id = note.Id,
            EquipeId = note.EquipeId,
            EquipeName = equipe.NameFr,
            CreatedByAdminId = note.CreatedByAdminId,
            CreatedByAdminName = admin.FullName,
            Content = note.Content,
            IsPrivate = note.IsPrivate,
            Created = DateTime.UtcNow
        };
    }
}
