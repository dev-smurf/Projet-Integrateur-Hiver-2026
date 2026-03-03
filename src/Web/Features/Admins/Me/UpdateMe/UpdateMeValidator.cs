using FastEndpoints;
using FluentValidation;

namespace Web.Features.Admins.Me.UpdateMe;

public class UpdateMeValidator : Validator<UpdateMeRequest>
{
    public UpdateMeValidator()
    {
        RuleFor(x => x.FirstName)
            .NotNull()
            .NotEmpty()
            .WithErrorCode("InvalidFirstName")
            .WithMessage("First name should not be empty.");

        RuleFor(x => x.LastName)
            .NotNull()
            .NotEmpty()
            .WithErrorCode("InvalidLastName")
            .WithMessage("Last name should not be empty.");
    }
}
