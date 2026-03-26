using Application.Interfaces.Services.Members;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Dtos;

namespace Web.Features.Members.Modules.GetMyModules;

public class GetMyModulesEndpoint : EndpointWithoutRequest<List<MemberModuleDto>>
{
    private readonly IMemberRepository _memberRepository;
    private readonly IAuthenticatedMemberService _authenticatedMemberService;

    public GetMyModulesEndpoint(
        IMemberRepository memberRepository,
        IAuthenticatedMemberService authenticatedMemberService)
    {
        _memberRepository = memberRepository;
        _authenticatedMemberService = authenticatedMemberService;
    }

    public override void Configure()
    {
        Get("members/me/modules");
        Roles(Domain.Constants.User.Roles.MEMBER);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var member = _authenticatedMemberService.GetAuthenticatedMember();
        var memberModules = await _memberRepository.GetMemberModules(member.Id);

        var response = memberModules.Select(mm => new MemberModuleDto
        {
            ModuleId = mm.ModuleId.ToString(),
            NameFr = mm.Module.NameFr,
            NameEn = mm.Module.NameEn,
            SujetFr = mm.Module.SujetFr,
            SujetEn = mm.Module.SujetEn,
            CardImageUrl = mm.Module.CardImageUrl,
            ProgressPercent = mm.ProgressPercent,
            IsCompleted = mm.IsCompleted
        }).ToList();

        await Send.OkAsync(response, ct);
    }
}
