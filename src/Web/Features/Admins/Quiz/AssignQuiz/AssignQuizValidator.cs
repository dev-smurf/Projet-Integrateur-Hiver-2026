using FluentValidation;

namespace Web.Features.Admins.Quiz.AssignQuiz;

public class AssignQuizValidator : AbstractValidator<AssignQuizRequest>
{
    public AssignQuizValidator()
    {
        RuleFor(x => x.QuizId)
            .NotEmpty()
            .WithMessage("L'ID du quiz est requis | Quiz ID is required");

        RuleFor(x => x.UserIds)
            .NotEmpty()
            .WithMessage("Au moins un utilisateur doit être sélectionné | At least one user must be selected")
            .Must(u => u.Count > 0)
            .WithMessage("Au moins un utilisateur doit être sélectionné | At least one user must be selected");
    }
}
