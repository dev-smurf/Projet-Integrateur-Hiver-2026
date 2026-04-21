using Application.Extensions;
using Application.Interfaces.Services.Notifications;
using Application.Settings;
using Domain.Common;
using Domain.Repositories;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Web.Features.Common;

namespace Web.Features.Public.Authentication.ForgotPassword;

public class ForgotPasswordEndpoint : EndpointWithSanitizedRequest<ForgotPasswordRequest, SucceededOrNotResponse>
{
    private readonly string _baseUrl;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ForgotPasswordEndpoint> _logger;
    private readonly INotificationService _notificationService;

    public ForgotPasswordEndpoint(
        IUserRepository userRepository,
        ILogger<ForgotPasswordEndpoint> logger,
        INotificationService notificationService,
        IOptions<ApplicationSettings> applicationSettings)
    {
        _logger = logger;
        _userRepository = userRepository;
        _notificationService = notificationService;
        _baseUrl = applicationSettings.Value.BaseUrl;
    }

    public override void Configure()
    {
        DontCatchExceptions();

        Post("authentication/forgot-password");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ForgotPasswordRequest req, CancellationToken ct)
    {
        var user = _userRepository.FindByUserName(req.Username);
        if (user == null)
        {
            _logger.LogInformation("Could not send reset password email since no user with email {email} exists.", req.Username);
            await Send.OkAsync(
                new SucceededOrNotResponse(
                    false,
                    new Error("UserNotFound", "Aucun compte n'est associe a cette adresse courriel.")),
                ct);
            return;
        }

        var token = await _userRepository.GetResetPasswordTokenForUser(user);
        var baseUrlNormalized = (_baseUrl ?? string.Empty).TrimEnd('/');
        var resetPath = (req.ResetPasswordRelativeUrl ?? string.Empty).Trim();

        string resetBaseUrl;
        if (Uri.TryCreate(resetPath, UriKind.Absolute, out var absoluteResetUrl))
        {
            resetBaseUrl = absoluteResetUrl.ToString();
        }
        else
        {
            if (string.IsNullOrWhiteSpace(baseUrlNormalized))
            {
                _logger.LogError("Application BaseUrl is missing; cannot build reset password link.");
                await Send.OkAsync(new SucceededOrNotResponse(false, new Error("InvalidResetPasswordRelativeUrl", "Reset password relative path should not be empty.")), ct);
                return;
            }

            var normalizedResetPath = resetPath.StartsWith("/") ? resetPath : "/" + resetPath;
            resetBaseUrl = baseUrlNormalized + normalizedResetPath;
        }

        var link = QueryHelpers.AddQueryString(resetBaseUrl, new Dictionary<string, string>
        {
            { "userId", user.Id.ToString() },
            { "token", token.Base64UrlEncode() }
        });

        var response = await _notificationService.SendForgotPasswordNotification(user, link);

        await Send.OkAsync(new SucceededOrNotResponse(response.Succeeded, response.Errors), ct);
    }

}
