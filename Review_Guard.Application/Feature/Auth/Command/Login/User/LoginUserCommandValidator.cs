using FluentValidation;
using Review_Guard.Application.Common.CommonMessages;

namespace Review_Guard.Application.Feature.Auth.Command.Login.User;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
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
