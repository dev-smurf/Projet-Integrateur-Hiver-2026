using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Dtos;

namespace Web.Features.Admins.Members.GetMemberModules;

public class GetMemberModulesEndpoint : Endpoint<GetMemberModulesRequest, List<MemberModuleDto>>
{
    private readonly IMemberRepository _memberRepository;

    public GetMemberModulesEndpoint(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public override void Configure()
    {
        DontCatchExceptions();
        Get("members/{memberId}/modules");

        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(GetMemberModulesRequest req, CancellationToken ct)
    {
        var memberModules = await _memberRepository.GetMemberModules(req.MemberId);
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
