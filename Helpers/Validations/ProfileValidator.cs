using Farma_api.Dto.Profile;
using FluentValidation;

namespace Farma_api.Helpers.Validations;

public class ChangeRequestValidator : AbstractValidator<ChangeRequest>
{
    public ChangeRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required");
        RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("Current password is required");
        RuleFor(x => x.NewPassword).NotEmpty().WithMessage("New password is required");
        RuleFor(x => x.NewPassword).MinimumLength(6).WithMessage("The new password must be at least 6 characters long");
        RuleFor(x => x.NewPassword).MaximumLength(20)
            .WithMessage("The new password must be at most 20 characters long");
    }
}