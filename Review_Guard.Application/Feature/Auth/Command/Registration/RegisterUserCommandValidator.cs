using FluentValidation;
using Review_Guard.Application.Common.CommonMessages;

namespace Review_Guard.Application.Feature.Auth.Command.Registration;

public class RegisterUserCommandValidator : AbstractValidator<RegistrationRegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.RegisterUserDto.FullName)
            .NotEmpty()
            .WithMessage(ValidationMessage.FullNameRequired);

        RuleFor(x => x.RegisterUserDto.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ValidationMessage.EmailRequired)
            .EmailAddress()
            .WithMessage(ValidationMessage.InvalidEmail);

        RuleFor(x => x.RegisterUserDto.Password)
            .NotEmpty()
            .WithMessage(ValidationMessage.PasswordRequired);

        RuleFor(x => x.RegisterUserDto.ConfirmPassword)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ValidationMessage.ConfirmPasswordRequired)
            .Equal(x => x.RegisterUserDto.Password)
            .WithMessage(AuthMessage.PasswordsDoNotMatch);
    }
}
