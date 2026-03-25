using FastEndpoints;
using FluentValidation;

namespace Web.Features.Admins.Members.GetMemberModules;

public class GetMemberModulesValidator : Validator<GetMemberModulesRequest>
{
    public GetMemberModulesValidator()
    {
        RuleFor(x => x.MemberId)
            .NotEqual(Guid.Empty)
            .WithErrorCode("InvalidMemberId")
            .WithMessage("Member id should not be null or empty.");
    }
}
