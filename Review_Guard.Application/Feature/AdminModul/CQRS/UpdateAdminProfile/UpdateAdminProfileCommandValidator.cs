// FILE: Review_Guard.Application / Feature / AdminModule / CQRS /
//       UpdateAdminProfile / UpdateAdminProfileCommandValidator.cs

using FluentValidation;

namespace Review_Guard.Application.Feature.AdminModule.CQRS.UpdateAdminProfile;

public sealed class UpdateAdminProfileCommandValidator
    : AbstractValidator<UpdateAdminProfileCommand>
{
    public UpdateAdminProfileCommandValidator()
    {
        RuleFor(x => x.AdminId).NotEmpty();

        When(x => x.Request.FullName is not null, () =>
            RuleFor(x => x.Request.FullName)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Full name must be between 1 and 100 characters."));
    }
}
