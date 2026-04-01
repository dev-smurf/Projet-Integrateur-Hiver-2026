using Application.Interfaces.Services.Users;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Members.Modules.MarkSectionRead;

public class MarkSectionReadEndpoint : EndpointWithoutRequest
{
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly IMemberRepository _memberRepository;
    private readonly IMemberModuleRepository _memberModuleRepository;

    public MarkSectionReadEndpoint(
        IAuthenticatedUserService authenticatedUserService,
        IMemberRepository memberRepository,
        IMemberModuleRepository memberModuleRepository)
    {
        _authenticatedUserService = authenticatedUserService;
        _memberRepository = memberRepository;
        _memberModuleRepository = memberModuleRepository;
    }

    public override void Configure()
    {
        Post("member/modules/{moduleId}/sections/{sectionId}/read");
        Roles(Domain.Constants.User.Roles.MEMBER);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var moduleIdStr = Route<string>("moduleId");
        var sectionIdStr = Route<string>("sectionId");

        if (!Guid.TryParse(moduleIdStr, out var moduleId) || !Guid.TryParse(sectionIdStr, out var sectionId))
        {
            HttpContext.Response.StatusCode = 400;
            return;
        }

        var user = _authenticatedUserService.GetAuthenticatedUser();
        var member = _memberRepository.FindByUserId(user.Id);

        if (member is null)
        {
            HttpContext.Response.StatusCode = 404;
            return;
        }

        var isAssigned = await _memberModuleRepository.IsAssignedAsync(member.Id, moduleId);
        if (!isAssigned)
        {
            HttpContext.Response.StatusCode = 403;
            return;
        }

        await _memberModuleRepository.MarkSectionReadAsync(member.Id, moduleId, sectionId);
        HttpContext.Response.StatusCode = 204;
    }
}
