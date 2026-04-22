namespace Web.Features.Members.Quiz.GetUserQuizResponses;

public class ScaleOption
{
    public int Value { get; set; }
    public string Label { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
}

public class MultipleChoiceOption
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
}

public class QuestionResponseDto
{
    public int QuestionNumber { get; set; }
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;

    // Pour Scale1To10
    public string? ScaleMinLabel { get; set; }
    public string? ScaleMidLabel { get; set; }
    public string? ScaleMaxLabel { get; set; }
    public List<ScaleOption> ScaleOptions { get; set; } = [];
    public int? SelectedScore { get; set; }

    // Pour MultipleChoice
    public List<MultipleChoiceOption> Options { get; set; } = [];
    public Guid? SelectedResponseId { get; set; }
    public string? SelectedResponseText { get; set; }

    // Pour TextInput
    public string? SelectedTextResponse { get; set; }
    public string? Placeholder { get; set; }
}

public class GetUserQuizResponsesResponse
{
    public Guid QuizId { get; set; }
    public string QuizTitle { get; set; } = string.Empty;
    public List<QuestionResponseDto> Responses { get; set; } = [];
    public int TotalQuestions { get; set; }
    public int AnsweredQuestions { get; set; }
    public bool IsComplete { get; set; }
}
