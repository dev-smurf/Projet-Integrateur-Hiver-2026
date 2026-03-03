namespace Application.Services.Notifications.Models;

public class ForgotPasswordNotificationModel : NotificationModel
{
    public string Link { get; set; }

    public ForgotPasswordNotificationModel(string destination, string locale, string link)
        : base(destination, locale)
    {
        Link = link;
    }

    public override string TemplateId()
    {
        if (Locale == "fr")
            return "d-e0eb0eb2bd2d40e1a167fa08c71b3e1d";
        return "d-b6252ba68bc44a1bb5bf8ae759c3f8d5";
    }

    public override object TemplateData()
    {
        return new
        {
            ButtonUrl = Link
        };
    }
}