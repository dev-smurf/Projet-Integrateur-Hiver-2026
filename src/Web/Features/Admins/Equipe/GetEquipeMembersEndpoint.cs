using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Application.Interfaces.Services.Equipe.Dto;

namespace Web.Features.Admins.Equipe;

public class GetEquipeMembersEndpoint : Endpoint<GetEquipeMembersRequest, GetEquipeMembersResponse>
{
    private readonly IMemberEquipeRepository _memberEquipeRepository;

    public GetEquipeMembersEndpoint(IMemberEquipeRepository memberEquipeRepository)
    {
        _memberEquipeRepository = memberEquipeRepository;
    }

    public override void Configure()
    {
        Get("equipes/{equipeId}/members");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        DontCatchExceptions();
    }

    public override async Task HandleAsync(GetEquipeMembersRequest req, CancellationToken ct)
    {
        if (!Guid.TryParse(req.EquipeId, out var equipeId))
        {
            await SendErrorsAsync(400, cancellation: ct);
            return;
        }

        var assignments = await _memberEquipeRepository.GetByEquipeIdAsync(equipeId);

        var response = new GetEquipeMembersResponse
        {
            EquipeId = req.EquipeId,
            Members = assignments.Select(me => new MemberEquipeDto
            {
                Id = me.Id.ToString(),
                MemberId = me.MemberId.ToString(),
                EquipeId = me.EquipeId.ToString(),
                Firstname = me.Member.FirstName,
                Lastname = me.Member.LastName,
                Email = me.Member.Email
            }).ToList()
        };

        await Send.OkAsync(response, cancellation: ct);
    }

    private async Task SendErrorsAsync(int v, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }
}

public class GetEquipeMembersRequest
{
    public string EquipeId { get; set; } = null!;
}

public class GetEquipeMembersResponse
{
    public string EquipeId { get; set; } = null!;
    public List<MemberEquipeDto> Members { get; set; } = new();
}