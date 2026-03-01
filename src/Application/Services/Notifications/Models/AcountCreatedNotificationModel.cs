namespace Application.Services.Notifications.Models;

public class AcountCreatedNotificationModel : NotificationModel
{
    public string Link { get; set; }
    public string FirstName { get; set; }

    public AcountCreatedNotificationModel(string firstname, string destination, string locale, string link)
        : base(destination, locale)
    {
        Link = link;
        FirstName = firstname;
    }

    public override string TemplateId()
    {
        if (Locale == "fr")
            return "d-29925896bf174284a4e8039d24f711fb";
        return "d-6123de7bc22d450bb7a4722047d789c3";
    }

    public override object TemplateData()
    {
        return new
        {
            ButtonUrl = Link,
            Username = FirstName
        };
    }
}