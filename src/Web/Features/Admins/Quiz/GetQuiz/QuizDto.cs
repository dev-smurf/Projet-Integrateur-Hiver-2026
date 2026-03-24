using Domain.Common;
using Domain.Entities;
using Domain.Extensions;

namespace Web.Features.Admins.Quiz.GetQuiz;

public class QuizDto
{
    public Guid Id { get; set; }
    public string Titre { get; set; } = null!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public List<QuizQuestionDto> Questions { get; set; } = new();
}

public class QuizQuestionDto
{
    public Guid Id { get; set; }
    public string QuestionText { get; set; } = null!;
    public int Order { get; set; }
    public QuizQuestionType QuestionType { get; set; }
    public TranslatableString QuestionTypeDisplay { get; set; } = null!;
    public string? Placeholder { get; set; }
    public string ScaleMinLabel { get; set; } = "Jamais";
    public string ScaleMidLabel { get; set; } = "Parfois";
    public string ScaleMaxLabel { get; set; } = "Toujours";
    public List<QuizResponseColumnDto> Responses { get; set; } = new();
}

public class QuizResponseColumnDto
{
    public Guid Id { get; set; }
    public string ResponseText { get; set; } = null!;
    public int Order { get; set; }
}

