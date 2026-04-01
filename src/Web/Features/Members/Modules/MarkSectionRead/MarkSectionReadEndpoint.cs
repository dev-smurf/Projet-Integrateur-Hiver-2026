using Application.Interfaces.Services.Users;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Members.Modules.MarkSectionRead;

public class MarkSectionReadRequest
{
    public string ModuleId { get; set; } = null!;
    public string SectionId { get; set; } = null!;
}

public class MarkSectionReadEndpoint : Endpoint<MarkSectionReadRequest>
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
        Post("member/modules/{ModuleId}/sections/{SectionId}/read");
        Roles(Domain.Constants.User.Roles.MEMBER);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(MarkSectionReadRequest req, CancellationToken ct)
    {
        if (!Guid.TryParse(req.ModuleId, out var moduleId) || !Guid.TryParse(req.SectionId, out var sectionId))
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
