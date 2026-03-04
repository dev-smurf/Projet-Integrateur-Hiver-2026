using Application.Exceptions.Members;
using Application.Interfaces.Services.Users;
using Domain.Common;
using Domain.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Features.Common;

namespace Web.Features.Members.Me.UpdateMe;

public class UpdateMeEndpoint : EndpointWithSanitizedRequest<UpdateMeRequest, SucceededOrNotResponse>
{
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly IMemberRepository _memberRepository;

    public UpdateMeEndpoint(
        IAuthenticatedUserService authenticatedUserService,
        IMemberRepository memberRepository)
    {
        _authenticatedUserService = authenticatedUserService;
        _memberRepository = memberRepository;
    }

    public override void Configure()
    {
        DontCatchExceptions();

        Put("members/me");
        Roles(Domain.Constants.User.Roles.MEMBER);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(UpdateMeRequest req, CancellationToken ct)
    {
        var user = _authenticatedUserService.GetAuthenticatedUser();
        var member = _memberRepository.FindByUserId(user.Id, asNoTracking: false);
        if (member == null)
            throw new MemberNotFoundException($"Could not find member for user {user.Id}");

        member.SetFirstName(req.FirstName);
        member.SetLastName(req.LastName);
        member.SetApartment(req.Apartment);
        member.SetStreet(req.Street);
        member.SetCity(req.City);
        member.SetZipCode(req.ZipCode);

        member.User.PhoneNumber = req.PhoneNumber;
        member.User.PhoneExtension = req.PhoneExtension;

        member.SanitizeForSaving();
        await _memberRepository.Update(member);

        await Send.OkAsync(new SucceededOrNotResponse(true), cancellation: ct);
    }
}
