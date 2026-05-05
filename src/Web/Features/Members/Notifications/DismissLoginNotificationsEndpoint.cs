using Application.Interfaces.Services.Members;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Members.Notifications;

public class DismissLoginNotificationsEndpoint : EndpointWithoutRequest<EmptyResponse>
{
    private readonly IAuthenticatedMemberService _authenticatedMemberService;
    private readonly IMemberRepository _memberRepository;

    public DismissLoginNotificationsEndpoint(
        IAuthenticatedMemberService authenticatedMemberService,
        IMemberRepository memberRepository)
    {
        _authenticatedMemberService = authenticatedMemberService;
        _memberRepository = memberRepository;
    }

    public override void Configure()
    {
        Post("members/me/login-notifications/dismiss");
        Roles(Domain.Constants.User.Roles.MEMBER);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        DontCatchExceptions();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var authenticated = _authenticatedMemberService.GetAuthenticatedMember();

        // Re-load with tracking so the change persists.
        var member = _memberRepository.FindByUserId(authenticated.User.Id, asNoTracking: false);
        if (member == null)
        {
            await Send.NoContentAsync(ct);
            return;
        }

        member.MarkNotificationsSeen();
        await _memberRepository.Update(member);

        await Send.NoContentAsync(ct);
    }
}
