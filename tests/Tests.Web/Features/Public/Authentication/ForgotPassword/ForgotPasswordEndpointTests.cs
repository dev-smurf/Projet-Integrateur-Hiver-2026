using Application.Interfaces.Services.Notifications;
using Application.Settings;
using Domain.Common;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tests.Common.Builders;
using Web.Features.Public.Authentication.ForgotPassword;

namespace Tests.Web.Features.Public.Authentication.ForgotPassword;

public class ForgotPasswordEndpointTests
{
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<ILogger<ForgotPasswordEndpoint>> _logger;
    private readonly Mock<INotificationService> _notificationService;
    private readonly ForgotPasswordEndpoint _endpoint;
    private readonly UserBuilder _userBuilder;

    public ForgotPasswordEndpointTests()
    {
        _userBuilder = new UserBuilder();
        _userRepository = new Mock<IUserRepository>();
        _logger = new Mock<ILogger<ForgotPasswordEndpoint>>();
        _notificationService = new Mock<INotificationService>();

        _endpoint = Factory.Create<ForgotPasswordEndpoint>(
            _userRepository.Object,
            _logger.Object,
            _notificationService.Object,
            Options.Create(new ApplicationSettings
            {
                BaseUrl = "https://example.com",
                RedirectUrl = "https://example.com",
                ErrorNotificationDestination = string.Empty
            }));
    }

    [Fact]
    public async Task GivenUserDoesNotExist_WhenHandleAsync_ThenReturnUserNotFoundError()
    {
        // Arrange
        var request = new ForgotPasswordRequest
        {
            Username = "missing@example.com",
            ResetPasswordRelativeUrl = "/reset-password"
        };

        _userRepository
            .Setup(x => x.FindByUserName(request.Username))
            .Returns((Domain.Entities.Identity.User?)null);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.Response.Succeeded.ShouldBeFalse();
        _endpoint.Response.Errors.ShouldContain(x =>
            x.ErrorType == "UserNotFound" &&
            x.ErrorMessage == "Aucun compte n'est associe a cette adresse courriel.");
        _notificationService.Verify(
            x => x.SendForgotPasswordNotification(It.IsAny<Domain.Entities.Identity.User>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task GivenUserExists_WhenHandleAsync_ThenDelegateSendingForgotPasswordNotification()
    {
        // Arrange
        var request = new ForgotPasswordRequest
        {
            Username = "john.doe@gmail.com",
            ResetPasswordRelativeUrl = "/reset-password"
        };
        var user = _userBuilder.WithId(Guid.NewGuid()).WithEmail(request.Username).Build();

        _userRepository.Setup(x => x.FindByUserName(request.Username)).Returns(user);
        _userRepository.Setup(x => x.GetResetPasswordTokenForUser(user)).ReturnsAsync("reset-token");
        _notificationService
            .Setup(x => x.SendForgotPasswordNotification(user, It.IsAny<string>()))
            .ReturnsAsync(new SucceededOrNotResponse(true));

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.Response.Succeeded.ShouldBeTrue();
        _notificationService.Verify(x => x.SendForgotPasswordNotification(user, It.IsAny<string>()), Times.Once);
    }
}
