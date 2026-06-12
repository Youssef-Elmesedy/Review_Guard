using FluentValidation;

namespace Review_Guard.Application.Feature.UserModul.Command.ChangePassword;

public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Request.CurrentPassword).NotEmpty();

        RuleFor(x => x.Request.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("New password must be at least 8 characters.");

        RuleFor(x => x.Request.ConfirmNewPassword)
            .Equal(x => x.Request.NewPassword)
            .WithMessage("Passwords do not match.");
    }
}
