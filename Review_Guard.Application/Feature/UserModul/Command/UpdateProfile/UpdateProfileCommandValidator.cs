using FluentValidation;

namespace Review_Guard.Application.Feature.UserModul.Command.UpdateProfile;

public sealed class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        When(x => x.Request.FullName is not null, () =>
            RuleFor(x => x.Request.FullName)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Full name must be between 1 and 100 characters."));
    }
}
