using FluentValidation;
using Review_Guard.Application.Common.CommonMessages;

namespace Review_Guard.Application.Feature.Auth.Command.Login.Admin;

public class LoginAdminCommandValidator : AbstractValidator<LoginAdminCommand>
{
    public LoginAdminCommandValidator()
    {
        RuleFor(x => x.LoginDto.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ValidationMessage.EmailRequired)
            .EmailAddress()
            .WithMessage(ValidationMessage.InvalidEmail);

        RuleFor(x => x.LoginDto.Password)
            .NotEmpty()
            .WithMessage(ValidationMessage.PasswordRequired);
    }
}
