using Domain.Helpers;
using FastEndpoints;
using FluentValidation;

namespace Web.Features.Admins.Members.UpdateMember;

public class UpdateMemberValidator : Validator<UpdateMemberRequest>
{
    public UpdateMemberValidator()
    {
        RuleFor(x => x.Id)
            .NotNull()
            .NotEmpty()
            .WithErrorCode("InvalidId")
            .WithMessage("Id should not be empty.");

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

        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .WithErrorCode("InvalidEmail")
            .WithMessage("Last name should not be empty.")
            .EmailAddress()
            .WithErrorCode("InvalidEmailFormat")
            .WithMessage("Email format is invalid.");

        RuleFor(x => x.PhoneNumber)
            .Must(x => string.IsNullOrWhiteSpace(x) || x.HasValidPhoneNumberFormat())
            .WithErrorCode("InvalidPhoneNumberFormat")
            .WithMessage("Phone number format is invalid.");

        RuleFor(x => x.ZipCode)
            .Must(x => string.IsNullOrWhiteSpace(x) || x.HasCanadianZipCodeFormat())
            .WithErrorCode("InvalidZipCodeFormat")
            .WithMessage("Zip code address format is not valid.");
    }
}
