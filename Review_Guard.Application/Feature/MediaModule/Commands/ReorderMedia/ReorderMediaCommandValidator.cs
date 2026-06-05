using FluentValidation;

namespace Review_Guard.Application.Feature.MediaModule.Commands.ReorderMedia;

public sealed class ReorderMediaCommandValidator : AbstractValidator<ReorderMediaCommand>
{
    public ReorderMediaCommandValidator()
    {
        RuleFor(x => x.OwnerId)
            .NotEmpty()
            .WithMessage("OwnerId is required.");

        RuleFor(x => x.OrderedIds)
            .NotNull()
            .NotEmpty()
            .WithMessage("At least one media ID must be provided.");

        RuleFor(x => x.OrderedIds)
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Duplicate media IDs are not allowed.");
    }
}
