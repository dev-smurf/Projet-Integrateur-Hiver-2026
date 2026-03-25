using FastEndpoints;
using FluentValidation;

namespace Web.Features.Admins.Members.AddMemberModule;

public class AddMemberModuleValidator : Validator<AddMemberModuleRequest>
{
    public AddMemberModuleValidator()
    {
        RuleFor(x => x.MemberId)
            .NotEqual(Guid.Empty)
            .WithErrorCode("InvalidMemberId")
            .WithMessage("Member id should not be null or empty.");

        RuleFor(x => x.ModuleId)
            .NotEqual(Guid.Empty)
            .WithErrorCode("InvalidModuleId")
            .WithMessage("Module id should not be null or empty.");
    }
}
