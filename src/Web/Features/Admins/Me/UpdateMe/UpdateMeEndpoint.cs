using Application.Exceptions.Admins;
using Application.Interfaces.Services.Users;
using Domain.Common;
using Domain.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Features.Common;

namespace Web.Features.Admins.Me.UpdateMe;

public class UpdateMeEndpoint : EndpointWithSanitizedRequest<UpdateMeRequest, SucceededOrNotResponse>
{
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly IAdministratorRepository _administratorRepository;

    public UpdateMeEndpoint(
        IAuthenticatedUserService authenticatedUserService,
        IAdministratorRepository administratorRepository)
    {
        _authenticatedUserService = authenticatedUserService;
        _administratorRepository = administratorRepository;
    }

    public override void Configure()
    {
        DontCatchExceptions();

        Put("admins/me");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(UpdateMeRequest req, CancellationToken ct)
    {
        var user = _authenticatedUserService.GetAuthenticatedUser();
        var admin = _administratorRepository.FindByUserId(user.Id, asNoTracking: false);
        if (admin == null)
            throw new AdministratorNotFoundException($"Could not find admin for user {user.Id}");

        admin.SetFirstName(req.FirstName);
        admin.SetLastName(req.LastName);

        admin.SanitizeForSaving();
        await _administratorRepository.Update(admin);

        await Send.OkAsync(new SucceededOrNotResponse(true), cancellation: ct);
    }
}
