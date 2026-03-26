using FluentValidation;

namespace Web.Features.Admins.Quiz.DeleteQuestion;

public class DeleteQuestionValidator : AbstractValidator<DeleteQuestionRequest>
{
    public DeleteQuestionValidator()
    {
        RuleFor(x => x.QuestionId)
            .NotEmpty()
            .WithMessage("Question ID is required");
    }
}
