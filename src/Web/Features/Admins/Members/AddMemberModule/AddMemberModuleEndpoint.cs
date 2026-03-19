using Domain.Common;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Members.AddMemberModule;

public class AddMemberModuleEndpoint : Endpoint<AddMemberModuleRequest, SucceededOrNotResponse>
{
    private readonly IMemberRepository _memberRepository;
    private readonly IModuleRepository _moduleRepository;

    public AddMemberModuleEndpoint(IMemberRepository memberRepository, IModuleRepository moduleRepository)
    {
        _memberRepository = memberRepository;
        _moduleRepository = moduleRepository;
    }

    public override void Configure()
    {
        DontCatchExceptions();

        Post("members/{memberId}/modules/{moduleId}");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(AddMemberModuleRequest req, CancellationToken ct)
    {
        var member = _memberRepository.FindById(req.MemberId);

        var module = await _moduleRepository.GetByIdAsync(req.ModuleId);
        if (module == null)
        {
            await Send.OkAsync(new SucceededOrNotResponse(false,
                new Error("ModuleNotFound", "Module not found.")), ct);
            return;
        }

        await _memberRepository.AddModuleToMember(member.Id, module.Id);

        await Send.OkAsync(new SucceededOrNotResponse(true), ct);
    }
}
