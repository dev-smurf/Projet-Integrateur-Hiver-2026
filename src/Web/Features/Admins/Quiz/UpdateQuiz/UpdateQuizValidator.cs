using FluentValidation;

namespace Web.Features.Admins.Quiz.UpdateQuiz;

public class UpdateQuizValidator : AbstractValidator<UpdateQuizRequest>
{
    public UpdateQuizValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Quiz ID is required");

        RuleFor(x => x.Titre)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(500)
            .WithMessage("Title must not exceed 500 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Description must not exceed 2000 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.ImageUrl)
            .MaximumLength(2000)
            .WithMessage("Image URL must not exceed 2000 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl));
    }
}
