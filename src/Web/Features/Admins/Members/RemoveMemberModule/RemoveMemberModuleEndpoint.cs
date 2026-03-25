using Domain.Common;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Members.RemoveMemberModule;

public class RemoveMemberModuleEndpoint : Endpoint<RemoveMemberModuleRequest, SucceededOrNotResponse>
{
    private readonly IMemberRepository _memberRepository;

    public RemoveMemberModuleEndpoint(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public override void Configure()
    {
        DontCatchExceptions();
        Delete("members/{memberId}/modules/{moduleId}");

        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(RemoveMemberModuleRequest req, CancellationToken ct)
    {
        await _memberRepository.RemoveModuleFromMember(req.MemberId, req.ModuleId);
        await Send.OkAsync(new SucceededOrNotResponse(true), ct);
    }
}
