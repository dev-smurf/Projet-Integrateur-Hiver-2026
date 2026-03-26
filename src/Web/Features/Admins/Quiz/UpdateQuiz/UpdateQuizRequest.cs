using Domain.Entities;

namespace Web.Features.Admins.Quiz.UpdateQuiz;

public class UpdateQuizRequest : Web.Features.Common.ISanitizable
{
    public Guid Id { get; set; }
    public string Titre { get; set; } = null!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public void Sanitize()
    {
        Titre = Titre.Trim();
        if (!string.IsNullOrWhiteSpace(Description))
            Description = Description.Trim();
        if (!string.IsNullOrWhiteSpace(ImageUrl))
            ImageUrl = ImageUrl.Trim();
    }
}

