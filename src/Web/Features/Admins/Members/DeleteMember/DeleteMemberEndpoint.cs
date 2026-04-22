using Application.Interfaces.Services;
using Domain.Repositories;
using FastEndpoints;
using Infrastructure.Repositories.Equipe;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Members.DeleteMember;

public class DeleteMemberEndpoint : Endpoint<DeleteMemberRequest, EmptyResponse>
{
    private readonly IMemberRepository _memberRepository;
    private readonly IEquipeRepository _equipeRepository;
    private readonly IMemberEquipeRepository _memberEquipeRepository;
    private readonly IHttpContextUserService _httpContextUserService;

    public DeleteMemberEndpoint(
        IMemberRepository memberRepository,
        IEquipeRepository equipeRepository,
        IMemberEquipeRepository memberEquipeRepository,
        IHttpContextUserService httpContextUserService)
    {
        _memberRepository = memberRepository;
        _equipeRepository = equipeRepository;
        _memberEquipeRepository = memberEquipeRepository;
        _httpContextUserService = httpContextUserService;
    }

    public override void Configure()
    {
        DontCatchExceptions();
        Delete("members/{id}");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(DeleteMemberRequest req, CancellationToken ct)
    {
        var member = _memberRepository.FindById(req.Id);

        var assignments = await _memberEquipeRepository.GetByMemberIdAsync(member.Id);
        foreach (var assignment in assignments)
        {
            await _memberEquipeRepository.UnassignAsync(assignment);
        }

        member.SoftDelete(_httpContextUserService.Username);
        await _memberRepository.Update(member);

        await Send.NoContentAsync(ct);
    }
}
