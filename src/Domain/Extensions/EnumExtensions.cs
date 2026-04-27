using Domain.Common;
using Domain.Entities;

namespace Domain.Extensions;

public static class EnumExtensions
{
    public static TranslatableString GetDisplayName(this QuizQuestionType questionType)
    {
        return questionType switch
        {
            QuizQuestionType.Scale1To10 => new TranslatableString("Échelle 1-10", "Scale 1-10"),
            QuizQuestionType.MultipleChoice => new TranslatableString("Choix unique", "Multiple Choice"),
            QuizQuestionType.TextInput => new TranslatableString("Texte libre", "Text Input"),
            QuizQuestionType.MultipleSelection => new TranslatableString("Choix multiples", "Multiple Selection"),
            _ => new TranslatableString("Inconnu", "Unknown")
        };
    }
}
