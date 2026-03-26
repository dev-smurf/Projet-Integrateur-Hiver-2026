using FluentValidation;

namespace Web.Features.Admins.Quiz.UpdateQuestion;

public class UpdateQuestionValidator : AbstractValidator<UpdateQuestionRequest>
{
    public UpdateQuestionValidator()
    {
        RuleFor(x => x.QuestionId)
            .NotEmpty()
            .WithMessage("Question ID is required");

        RuleFor(x => x.QuestionText)
            .NotEmpty()
            .WithMessage("Question text is required")
            .MaximumLength(1000)
            .WithMessage("Question text must not exceed 1000 characters");

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Order must be 0 or greater");

        RuleFor(x => x.QuestionType)
            .IsInEnum()
            .WithMessage("Invalid question type");

        RuleFor(x => x.Placeholder)
            .MaximumLength(500)
            .WithMessage("Placeholder must not exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Placeholder));

        RuleFor(x => x.Responses)
            .NotEmpty()
            .WithMessage("Question must have at least one response")
            .When(x => x.QuestionType != Domain.Entities.QuizQuestionType.TextInput);

        RuleForEach(x => x.Responses)
            .SetValidator(new UpdateResponseValidator());
    }
}

public class UpdateResponseValidator : AbstractValidator<UpdateResponseRequest>
{
    public UpdateResponseValidator()
    {
        RuleFor(x => x.ResponseText)
            .NotEmpty()
            .WithMessage("Response text is required")
            .MaximumLength(500)
            .WithMessage("Response text must not exceed 500 characters");

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Order must be 0 or greater");
    }
}
