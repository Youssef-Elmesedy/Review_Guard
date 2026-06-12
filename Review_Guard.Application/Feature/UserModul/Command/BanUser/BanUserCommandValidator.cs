using FluentValidation;

namespace Review_Guard.Application.Feature.UserModul.Command.BanUser;

public sealed class BanUserCommandValidator : AbstractValidator<BanUserCommand>
{
    public BanUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.AdminId).NotEmpty();
        RuleFor(x => x.Request.Reason).NotEmpty().MaximumLength(500);
    }
}
