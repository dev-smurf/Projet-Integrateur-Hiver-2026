using Domain.Helpers;
using FastEndpoints;
using FluentValidation;

namespace Web.Features.Members.Me.UpdateMe;

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

        When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber), () =>
        {
            RuleFor(x => x.PhoneNumber)
                .Must(x => x!.HasValidPhoneNumberFormat())
                .WithErrorCode("InvalidPhoneNumberFormat")
                .WithMessage("Phone number format is invalid.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.ZipCode), () =>
        {
            RuleFor(x => x.ZipCode)
                .Must(x => x!.HasCanadianZipCodeFormat())
                .WithErrorCode("InvalidZipCodeFormat")
                .WithMessage("Zip code format is not valid.");
        });
    }
}
