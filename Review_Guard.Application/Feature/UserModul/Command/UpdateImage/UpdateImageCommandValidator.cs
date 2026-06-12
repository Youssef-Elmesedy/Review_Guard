using FluentValidation;

namespace Review_Guard.Application.Feature.UserModul.Command.UpdateImage;

public class UpdateImageCommandValidator : AbstractValidator<UpdateImageCommand>
{
    public UpdateImageCommandValidator()
    {
        RuleFor(x => x.userId)
            .NotEmpty().WithMessage(UserMessage.UserIdRequired);
        RuleFor(x => x.fileImage)
            .NotNull().WithMessage(UserMessage.FileImageRequired);
    }
}
