using Application.Interfaces.Services.Members;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Dtos;

namespace Web.Features.Members.Notes.GetMyNotes;

public class GetMyNotesEndpoint : EndpointWithoutRequest<List<NoteDto>>
{
    private readonly IMemberNoteRepository _noteRepository;
    private readonly IAuthenticatedMemberService _authenticatedMemberService;

    public GetMyNotesEndpoint(
        IMemberNoteRepository noteRepository,
        IAuthenticatedMemberService authenticatedMemberService)
    {
        _noteRepository = noteRepository;
        _authenticatedMemberService = authenticatedMemberService;
    }

    public override void Configure()
    {
        Get("members/me/notes");
        Roles(Domain.Constants.User.Roles.MEMBER);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var member = _authenticatedMemberService.GetAuthenticatedMember();
        var allNotes = await _noteRepository.GetByMemberIdAsync(member.Id, ct);
        
        // Filter out private notes
        var publicNotes = allNotes.Where(n => !n.IsPrivate);

        var response = publicNotes.Select(n => new NoteDto
        {
            Id = n.Id,
            MemberId = n.MemberId,
            MemberName = n.Member?.FullName ?? string.Empty,
            CreatedByAdminId = n.CreatedByAdminId,
            CreatedByAdminName = n.CreatedByAdmin?.FullName ?? string.Empty,
            Content = n.Content,
            IsPrivate = n.IsPrivate,
            Created = n.Created.ToDateTimeUtc()
        }).ToList();

        Response = response;
    }
}
