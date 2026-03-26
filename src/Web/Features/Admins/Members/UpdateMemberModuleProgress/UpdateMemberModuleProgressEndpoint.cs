using Domain.Common;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Members.UpdateMemberModuleProgress;

public class UpdateMemberModuleProgressEndpoint : Endpoint<UpdateMemberModuleProgressRequest, SucceededOrNotResponse>
{
    private readonly IMemberRepository _memberRepository;

    public UpdateMemberModuleProgressEndpoint(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public override void Configure()
    {
        DontCatchExceptions();
        Put("members/{memberId}/modules/{moduleId}/progress");

        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(UpdateMemberModuleProgressRequest req, CancellationToken ct)
    {
        await _memberRepository.UpdateMemberModuleProgress(req.MemberId, req.ModuleId, req.ProgressPercent);
        await Send.OkAsync(new SucceededOrNotResponse(true), ct);
    }
}
