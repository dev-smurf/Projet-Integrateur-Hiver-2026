using FluentValidation;

namespace Web.Features.Admins.Quiz.AssignQuiz;

public class AssignQuizValidator : AbstractValidator<AssignQuizRequest>
{
    public AssignQuizValidator()
    {
        RuleFor(x => x.QuizId)
            .NotEmpty()
            .WithMessage("L'ID du quiz est requis | Quiz ID is required");

        RuleFor(x => x)
            .Must(x => x.UserIds.Count > 0 || x.EquipeIds.Count > 0)
            .WithMessage("Au moins un utilisateur ou une equipe doit etre selectionne | At least one user or team must be selected");

        RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(x => x.AvailableAt)
            .When(x => x.DueDate.HasValue && x.AvailableAt.HasValue)
            .WithMessage("La date limite doit etre apres la date de disponibilite | Due date must be after the availability date");

        RuleFor(x => x.FollowUpLabel)
            .MaximumLength(100)
            .WithMessage("Le nom du point de suivi doit contenir 100 caracteres ou moins | Follow-up label must be 100 characters or fewer");
    }
}
