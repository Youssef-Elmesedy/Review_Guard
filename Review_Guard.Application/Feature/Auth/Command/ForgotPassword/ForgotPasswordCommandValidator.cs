using FluentValidation;
using Review_Guard.Application.Common.CommonMessages;

namespace Review_Guard.Application.Feature.Auth.Command.ForgotPassword;

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.ForgotPasswordDto.Email)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .WithMessage(ValidationMessage.EmailRequired)
           .EmailAddress()
           .WithMessage(ValidationMessage.InvalidEmail);
    }
}
