using Application.Interfaces.Services.Module.Dto;
using Application.Interfaces.Services.Users;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Members.Modules;

public class GetMyModulesEndpoint : EndpointWithoutRequest<List<ModuleDto>>
{
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly IMemberRepository _memberRepository;
    private readonly IMemberModuleRepository _memberModuleRepository;

    public GetMyModulesEndpoint(
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
        Get("member/modules");
        Roles(Domain.Constants.User.Roles.MEMBER);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var user = _authenticatedUserService.GetAuthenticatedUser();
        var member = _memberRepository.FindByUserId(user.Id);

        if (member is null)
        {
            HttpContext.Response.StatusCode = 404;
            return;
        }

        var assignments = await _memberModuleRepository.GetByMemberIdAsync(member.Id);
        Response = assignments.Select(a => new ModuleDto
        {
            Id = a.Module.Id.ToString(),
            Name = a.Module.Name,
            Subject = a.Module.Subject,
            Content = a.Module.Content,
            CardImageUrl = a.Module.CardImageUrl
        }).ToList();
    }
}
