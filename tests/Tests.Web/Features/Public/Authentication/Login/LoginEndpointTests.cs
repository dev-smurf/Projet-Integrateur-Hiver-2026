using Application.Interfaces.Services.Notifications;
using Application.Interfaces.Services.Users;
using Application.Settings;
using Domain.Common;
using FastEndpoints;
using Microsoft.Extensions.Options;
using Tests.Common.Builders;
using Web.Features.Public.Authentication.Login;

namespace Tests.Web.Features.Public.Authentication.Login;

public class LoginEndpointTests
{
    private readonly Mock<INotificationService> _notificationService;
    private readonly Mock<IAuthenticationService> _authenticationService;
    private readonly LoginEndpoint _endpoint;
    private readonly UserBuilder _userBuilder;

    public LoginEndpointTests()
    {
        _userBuilder = new UserBuilder();
        _notificationService = new Mock<INotificationService>();
        _authenticationService = new Mock<IAuthenticationService>();

        _endpoint = Factory.Create<LoginEndpoint>(
            Options.Create(new CookieSettings
            {
                Domain = "localhost",
                Secure = false
            }),
            _notificationService.Object,
            _authenticationService.Object);
    }

    [Fact]
    public async Task GivenTwoFactorEmailFails_WhenHandleAsync_ThenReturnNotificationErrors()
    {
        // Arrange
        const string username = "john.doe@gmail.com";
        const string password = "Qwerty123!";
        const string code = "123456";
        var user = _userBuilder.WithEmail(username).Build();
        var notificationError = new Error("EmailDeliveryFailed", "Le code de connexion n'a pas pu etre envoye.");

        _authenticationService
            .Setup(x => x.FindUserWithUsernameAndPassword(username, password))
            .ReturnsAsync(user);
        _authenticationService
            .Setup(x => x.GetTwoFactorAuthenticationTokenCodeUserWithPassword(user, password))
            .ReturnsAsync(code);
        _notificationService
            .Setup(x => x.SendTwoFactorAuthenticationCodeNotification(user, code))
            .ReturnsAsync(new SucceededOrNotResponse(false, notificationError));

        var request = new LoginRequest
        {
            Username = username,
            Password = password
        };

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.Response.Succeeded.ShouldBeFalse();
        _endpoint.Response.Errors.ShouldContain(x =>
            x.ErrorType == notificationError.ErrorType &&
            x.ErrorMessage == notificationError.ErrorMessage);
    }
}
