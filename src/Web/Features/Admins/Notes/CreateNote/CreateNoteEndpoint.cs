using Application.Interfaces.Services.Admins;
using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Dtos;

namespace Web.Features.Admins.Notes.CreateNote;

public class CreateNoteEndpoint : Endpoint<CreateNoteRequest, NoteDto>
{
    private readonly IMemberNoteRepository _noteRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IAuthenticatedAdminService _adminService;

    public CreateNoteEndpoint(
        IMemberNoteRepository noteRepository,
        IMemberRepository memberRepository,
        IAuthenticatedAdminService adminService)
    {
        _noteRepository = noteRepository;
        _memberRepository = memberRepository;
        _adminService = adminService;
    }

    public override void Configure()
    {
        DontCatchExceptions();
        Post("admin/notes");

        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CreateNoteRequest req, CancellationToken ct)
    {
        var admin = _adminService.GetAuthenticatedAdmin();
        if (admin == null)
        {
            HttpContext.Response.StatusCode = 403;
            return;
        }

        var member = _memberRepository.FindById(req.MemberId);
        if (member == null)
        {
            HttpContext.Response.StatusCode = 404;
            return;
        }

        var note = new MemberNote(member.Id, admin.Id, req.Content, req.IsPrivate);
        await _noteRepository.AddAsync(note, ct);

        var dto = new NoteDto
        {
            Id = note.Id,
            MemberId = note.MemberId,
            MemberName = member.FullName,
            CreatedByAdminId = note.CreatedByAdminId,
            CreatedByAdminName = admin.FullName,
            Content = note.Content,
            IsPrivate = note.IsPrivate,
            Created = DateTime.UtcNow // approximate, since we don't reload from db
        };

        Response = dto;
    }
}
