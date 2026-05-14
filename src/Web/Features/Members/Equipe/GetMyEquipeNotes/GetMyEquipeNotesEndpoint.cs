using Application.Interfaces.Services.Members;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Dtos;
using System.Linq;

namespace Web.Features.Members.Equipe.GetMyEquipeNotes;

public class GetMyEquipeNotesEndpoint : EndpointWithoutRequest<IEnumerable<EquipeNoteDto>>
{
    private readonly IEquipeNoteRepository _equipeNoteRepository;
    private readonly IEquipeRepository _equipeRepository;
    private readonly IAuthenticatedMemberService _authenticatedMemberService;

    public GetMyEquipeNotesEndpoint(
        IEquipeNoteRepository equipeNoteRepository,
        IEquipeRepository equipeRepository,
        IAuthenticatedMemberService authenticatedMemberService)
    {
        _equipeNoteRepository = equipeNoteRepository;
        _equipeRepository = equipeRepository;
        _authenticatedMemberService = authenticatedMemberService;
    }

    public override void Configure()
    {
        Get("members/me/equipe-notes");
        Roles(Domain.Constants.User.Roles.MEMBER);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var member = _authenticatedMemberService.GetAuthenticatedMember();
        
        // Get all equipe IDs for the member
        var equipeIds = await _equipeRepository.GetEquipeIdsForUser(member.User.Id);
        
        var allTeamNotes = new List<Domain.Entities.EquipeNote>();
        
        foreach (var equipeId in equipeIds)
        {
            var notes = await _equipeNoteRepository.GetByEquipeIdAsync(equipeId, ct);
            // Only public notes
            allTeamNotes.AddRange(notes.Where(n => !n.IsPrivate));
        }

        var response = allTeamNotes
            .OrderByDescending(n => n.Created)
            .Select(n => new EquipeNoteDto
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

        Response = response;
    }
}
