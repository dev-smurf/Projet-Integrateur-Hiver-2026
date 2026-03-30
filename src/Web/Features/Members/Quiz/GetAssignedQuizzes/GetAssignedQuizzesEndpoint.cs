using Application.Interfaces.Services.Members;
using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Members.Quiz.GetAssignedQuizzes;

public class GetAssignedQuizzesEndpoint : Endpoint<EmptyRequest, List<AssignedQuizDto>>
{
    private readonly IQuizAssignmentRepository _assignmentRepository;
    private readonly IAuthenticatedMemberService _authenticatedMemberService;

    public GetAssignedQuizzesEndpoint(
        IQuizAssignmentRepository assignmentRepository,
        IAuthenticatedMemberService authenticatedMemberService)
    {
        _assignmentRepository = assignmentRepository;
        _authenticatedMemberService = authenticatedMemberService;
    }

    public override void Configure()
    {
        Get("quiz/assigned");
        Roles(Domain.Constants.User.Roles.MEMBER);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var member = _authenticatedMemberService.GetAuthenticatedMember();
        var userId = member.User.Id;

        var assignments = await _assignmentRepository.GetByUserIdAsync(userId);
        if (member.Id != userId)
        {
            var legacyAssignments = await _assignmentRepository.GetByUserIdAsync(member.Id);
            if (legacyAssignments.Count > 0)
            {
                var merged = assignments.Concat(legacyAssignments)
                    .GroupBy(a => a.Id)
                    .Select(g => g.First())
                    .ToList();
                assignments = merged;
            }
        }

        var result = assignments
            .Where(a => !a.Deleted.HasValue && a.Quiz is not null && !a.Quiz.Deleted.HasValue)
            .Select(a => new AssignedQuizDto
            {
                Id = a.Id,
                QuizId = a.QuizId,
                Titre = a.Quiz!.Titre,
                Description = a.Quiz!.Description,
                ImageUrl = a.Quiz!.ImageUrl,
                AssignedAt = a.AssignedAt,
                DueDate = a.DueDate,
                CompletedAt = a.CompletedAt
            })
            .ToList();

        await Send.OkAsync(result, cancellation: ct);
    }
}
