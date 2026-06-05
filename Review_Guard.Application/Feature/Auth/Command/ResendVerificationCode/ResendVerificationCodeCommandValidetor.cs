using FluentValidation;
using Review_Guard.Application.Common.CommonMessages;

namespace Review_Guard.Application.Feature.Auth.Command.ResendVerificationCode;

public class ResendVerificationCodeCommandValidetor : AbstractValidator<ResendVerificationCodeCommand>
{
    public ResendVerificationCodeCommandValidetor()
    {
        RuleFor(e => e.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ValidationMessage.EmailRequired)
            .EmailAddress()
            .WithMessage(ValidationMessage.InvalidEmail);

        RuleFor(p => p.password)
            .NotEmpty()
            .WithMessage(ValidationMessage.PasswordRequired);

        RuleFor(t => t.Type)
            .NotEmpty()
            .WithMessage(ValidationMessage.VerificationCodeType);
    }
}
