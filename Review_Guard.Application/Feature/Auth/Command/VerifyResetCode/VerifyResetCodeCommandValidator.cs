using FluentValidation;

namespace Review_Guard.Application.Feature.Auth.Command.VerifyResetCode;

public class VerifyResetCodeCommandValidator : AbstractValidator<VerifyResetCodeCommand>
{
    public VerifyResetCodeCommandValidator()
    {
        RuleFor(x => x.code)
            .NotEmpty().WithMessage("Reset code is required.")
            .Length(6).WithMessage("Reset code must be 6 characters long.");
    }
}
