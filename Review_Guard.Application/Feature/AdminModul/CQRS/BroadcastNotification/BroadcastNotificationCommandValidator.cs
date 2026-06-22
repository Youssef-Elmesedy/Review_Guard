// FILE: Review_Guard.Application / Feature / AdminModule / CQRS /
//       BroadcastNotification / BroadcastNotificationCommandValidator.cs

using FluentValidation;

namespace Review_Guard.Application.Feature.AdminModule.CQRS.BroadcastNotification;

public sealed class BroadcastNotificationCommandValidator
    : AbstractValidator<BroadcastNotificationCommand>
{
    public BroadcastNotificationCommandValidator()
    {
        RuleFor(x => x.AdminId).NotEmpty();

        RuleFor(x => x.Request.Title)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Request.Message)
            .NotEmpty()
            .MaximumLength(500);
    }
}
