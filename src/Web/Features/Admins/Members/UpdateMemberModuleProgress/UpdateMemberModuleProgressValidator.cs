using FastEndpoints;
using FluentValidation;

namespace Web.Features.Admins.Members.UpdateMemberModuleProgress;

public class UpdateMemberModuleProgressValidator : Validator<UpdateMemberModuleProgressRequest>
{
    public UpdateMemberModuleProgressValidator()
    {
        RuleFor(x => x.MemberId)
            .NotEqual(Guid.Empty)
            .WithErrorCode("InvalidMemberId")
            .WithMessage("Member id should not be null or empty.");

        RuleFor(x => x.ModuleId)
            .NotEqual(Guid.Empty)
            .WithErrorCode("InvalidModuleId")
            .WithMessage("Module id should not be null or empty.");

        RuleFor(x => x.ProgressPercent)
            .InclusiveBetween(0, 100)
            .WithErrorCode("InvalidProgressPercent")
            .WithMessage("Progress percent must be between 0 and 100.");
    }
}
