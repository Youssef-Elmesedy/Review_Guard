using FluentValidation;

namespace Review_Guard.Application.Feature.BusinessModul.Common.Command.RejectBusiness;

public sealed class RejectBusinessCommandValidator : AbstractValidator<RejectBusinessCommand>
{
    public RejectBusinessCommandValidator()
    {
        RuleFor(x => x.BusinessId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}
