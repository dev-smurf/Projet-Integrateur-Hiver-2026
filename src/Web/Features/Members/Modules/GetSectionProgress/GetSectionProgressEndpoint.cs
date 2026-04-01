using Application.Interfaces.Services.Users;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Dtos;

namespace Web.Features.Members.Modules.GetSectionProgress;

public class GetSectionProgressEndpoint : EndpointWithoutRequest<List<SectionProgressDto>>
{
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly IMemberRepository _memberRepository;
    private readonly IMemberModuleRepository _memberModuleRepository;

    public GetSectionProgressEndpoint(
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
        Get("member/modules/{moduleId}/sections/progress");
        Roles(Domain.Constants.User.Roles.MEMBER);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var moduleIdStr = Route<string>("moduleId");

        if (!Guid.TryParse(moduleIdStr, out var moduleId))
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

        var memberModule = await _memberModuleRepository.GetByMemberAndModuleAsync(member.Id, moduleId);
        if (memberModule is null)
        {
            HttpContext.Response.StatusCode = 403;
            return;
        }

        var sectionProgress = await _memberModuleRepository.GetSectionProgressAsync(memberModule.Id);

        Response = sectionProgress.Select(sp => new SectionProgressDto
        {
            SectionId = sp.ModuleSectionId.ToString(),
            IsRead = sp.IsRead
        }).ToList();
    }
}
