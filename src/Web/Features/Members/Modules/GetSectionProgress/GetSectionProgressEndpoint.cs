using Application.Interfaces.Services.Users;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Dtos;

namespace Web.Features.Members.Modules.GetSectionProgress;

public class GetSectionProgressRequest
{
    public string ModuleId { get; set; } = null!;
}

public class GetSectionProgressEndpoint : Endpoint<GetSectionProgressRequest, List<SectionProgressDto>>
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
        Get("member/modules/{ModuleId}/sections/progress");
        Roles(Domain.Constants.User.Roles.MEMBER);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(GetSectionProgressRequest req, CancellationToken ct)
    {
        if (!Guid.TryParse(req.ModuleId, out var moduleId))
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
            Response = new List<SectionProgressDto>();
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
