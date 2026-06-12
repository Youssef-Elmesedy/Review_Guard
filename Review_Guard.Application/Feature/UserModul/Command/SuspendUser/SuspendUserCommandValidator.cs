using FluentValidation;

namespace Review_Guard.Application.Feature.UserModul.Command.SuspendUser;

public sealed class SuspendUserCommandValidator : AbstractValidator<SuspendUserCommand>
{
    public SuspendUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.AdminId).NotEmpty();
        RuleFor(x => x.Request.Reason).NotEmpty().MaximumLength(500);

        When(x => x.Request.SuspendedUntil.HasValue, () =>
            RuleFor(x => x.Request.SuspendedUntil)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("SuspendedUntil must be a future date."));
    }
}
