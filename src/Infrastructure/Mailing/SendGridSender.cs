using System.Text.Json;
using Application.Interfaces.Mailing;
using Application.Services.Notifications.Models;
using Domain.Common;
using Microsoft.Extensions.Logging;
using SendGrid;

namespace Infrastructure.Mailing;

public class SendGridSender : IEmailSender
{
    private readonly ILogger<SendGridSender> _logger;
    private readonly ISendGridClient _sendGridClient;
    private readonly ISendGridMessageFactory _sendGridMessageFactory;

    public SendGridSender(
        ILogger<SendGridSender> logger,
        ISendGridClient sendGridClient,
        ISendGridMessageFactory sendGridMessageFactory)
    {
        _logger = logger;
        _sendGridClient = sendGridClient;
        _sendGridMessageFactory = sendGridMessageFactory;
    }

    public async Task<SucceededOrNotResponse> SendAsync<TModel>(TModel model) where TModel : NotificationModel
    {
        var msg = _sendGridMessageFactory.CreateFromModel(model);
        var response = await _sendGridClient.SendEmailAsync(msg);

        if (response.IsSuccessStatusCode)
            return new SucceededOrNotResponse(response.IsSuccessStatusCode);

        var errors = await GetErrorListFromResponse(response);
        _logger.LogError("Error occured while sending email. Errors : {errors}", JsonSerializer.Serialize(errors));

        return new SucceededOrNotResponse(response.IsSuccessStatusCode, errors);
    }

    private async Task<List<Error>> GetErrorListFromResponse(Response response)
    {
        var body = await response.Body.ReadAsStringAsync();

        if (string.IsNullOrEmpty(body))
            return new List<Error>();

        try
        {
            var sendGridErrors = JsonSerializer.Deserialize<List<SendGridError>>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return sendGridErrors?.Select(x => new Error("SendGridError", x.Message)).ToList()
                ?? new List<Error>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize SendGrid error response. Raw body: {body}", body);
            return new List<Error> { new Error("SendGridError", body) };
        }
    }
}