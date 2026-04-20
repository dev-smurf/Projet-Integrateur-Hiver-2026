using Application.Services.Notifications.Models;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;

namespace Infrastructure.Mailing;

public class SendGridMessageFactory : ISendGridMessageFactory
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly MailingSettings _mailingSettings;
    private readonly IMapper _mapper;

    public SendGridMessageFactory(IWebHostEnvironment webHostEnvironment, IOptions<MailingSettings> mailingSettings, IMapper mapper)
    {
        _mapper = mapper;
        _webHostEnvironment = webHostEnvironment;
        _mailingSettings = mailingSettings.Value;
    }

    public SendGridMessage CreateFromModel<TModel>(TModel model) where TModel : NotificationModel
    {
        var lang = _webHostEnvironment.IsProduction() ? "Fr" : "Fr"; // Default to French
        var fromName = _mailingSettings.FromName.GetValueOrDefault(lang, "Garneau");
        var fromAddress = _mailingSettings.FromAddress.GetValueOrDefault(lang, "noreply@garneau.com");

        var msg = new SendGridMessage
        {
            From = new EmailAddress(fromAddress, fromName),
            TemplateId = model.TemplateId()
        };

        if (model.Attachments.Any())
            msg.Attachments = model.Attachments.Select(x => _mapper.Map<Attachment>(x)).ToList();

        msg.SetTemplateData(model.TemplateData());

        if (_webHostEnvironment.IsProduction())
            msg.AddTo(model.Destination);
        else if (_webHostEnvironment.IsDevelopment())
            msg.AddTo(_mailingSettings.ToAddressForDevelopment);

        return msg;
    }
}