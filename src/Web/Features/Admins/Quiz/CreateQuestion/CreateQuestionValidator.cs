using FluentValidation;

namespace Web.Features.Admins.Quiz.CreateQuestion;

public class CreateQuestionValidator : AbstractValidator<CreateQuestionRequest>
{
    public CreateQuestionValidator()
    {
        RuleFor(x => x.QuizId)
            .NotEmpty()
            .WithMessage("L'ID du quiz est requis | Quiz ID is required");

        RuleFor(x => x.QuestionText)
            .NotEmpty()
            .WithMessage("Le texte de la question est requis | Question text is required")
            .MaximumLength(1000)
            .WithMessage("Le texte de la question ne doit pas dépasser 1000 caractères | Question text must not exceed 1000 characters");

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0)
            .WithMessage("L'ordre doit être 0 ou supérieur | Order must be 0 or greater");

        RuleFor(x => x.QuestionType)
            .IsInEnum()
            .WithMessage("Type de question invalide | Invalid question type");

        RuleFor(x => x.Placeholder)
            .MaximumLength(500)
            .WithMessage("L'espace réservé ne doit pas dépasser 500 caractères | Placeholder must not exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Placeholder));

        RuleFor(x => x.Responses)
            .NotEmpty()
            .WithMessage("La question doit avoir au moins une réponse | Question must have at least one response")
            .When(x => x.QuestionType != Domain.Entities.QuizQuestionType.TextInput);

        RuleForEach(x => x.Responses)
            .SetValidator(new ResponseValidator());
    }
}

public class ResponseValidator : AbstractValidator<CreateResponseRequest>
{
    public ResponseValidator()
    {
        RuleFor(x => x.ResponseText)
            .NotEmpty()
            .WithMessage("Le texte de la réponse est requis | Response text is required")
            .MaximumLength(500)
            .WithMessage("Le texte de la réponse ne doit pas dépasser 500 caractères | Response text must not exceed 500 characters");

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0)
            .WithMessage("L'ordre doit être 0 ou supérieur | Order must be 0 or greater");
    }
}
