using FluentValidation;
using Domain.Entities;

namespace Web.Features.Admins.Quiz.CreateQuiz;

public class CreateQuizValidator : AbstractValidator<CreateQuizRequest>
{
    public CreateQuizValidator()
    {
        RuleFor(x => x.Titre)
            .NotEmpty().WithMessage("Quiz title is required")
            .MaximumLength(255).WithMessage("Quiz title must not exceed 255 characters");

        RuleFor(x => x.Questions)
            .NotEmpty().WithMessage("Quiz must have at least one question")
            .Must(q => q.Count > 0).WithMessage("At least one question is required");

        RuleForEach(x => x.Questions).SetValidator(new CreateQuizQuestionValidator());
    }
}

public class CreateQuizQuestionValidator : AbstractValidator<CreateQuizQuestionRequest>
{
    public CreateQuizQuestionValidator()
    {
        RuleFor(x => x.QuestionText)
            .NotEmpty().WithMessage("Question text is required");

        RuleFor(x => x.QuestionType)
            .IsInEnum().WithMessage("Invalid question type");

        RuleFor(x => x.Responses)
            .NotEmpty().WithMessage("Question must have responses")
            .Must(r => r.Count > 0).WithMessage("At least one response is required")
            .When(x => x.QuestionType == QuizQuestionType.MultipleChoice);

        RuleForEach(x => x.Responses).SetValidator(new CreateQuizResponseValidator());
    }
}

public class CreateQuizResponseValidator : AbstractValidator<CreateQuizResponseRequest>
{
    public CreateQuizResponseValidator()
    {
        RuleFor(x => x.ResponseText)
            .NotEmpty().WithMessage("Le texte de la réponse est requis | Response text is required");
    }
}
