using Domain.Entities;

namespace Web.Features.Admins.Quiz.CreateQuiz;

public class CreateQuizRequest
{
    public string Titre { get; set; } = null!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public List<CreateQuizQuestionRequest> Questions { get; set; } = new();
}

public class CreateQuizQuestionRequest
{
    public string QuestionText { get; set; } = null!;
    public int Order { get; set; }
    public QuizQuestionType QuestionType { get; set; } = QuizQuestionType.Scale1To10;
    public string? Placeholder { get; set; } // For TextInput questions
    public string ScaleMinLabel { get; set; } = "Jamais";
    public string ScaleMidLabel { get; set; } = "Parfois";
    public string ScaleMaxLabel { get; set; } = "Toujours";
    public List<CreateQuizResponseRequest> Responses { get; set; } = new();
}

public class CreateQuizResponseRequest
{
    public string ResponseText { get; set; } = null!;
    public int Order { get; set; }
}

