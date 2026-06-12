using FluentValidation;
namespace Review_Guard.Application.Feature.ProofModul.Command.SubmitProofByOrder;
public sealed class SubmitProofByOrderCommandValidator : AbstractValidator<SubmitProofByOrderCommand>
{
    public SubmitProofByOrderCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Request.BranchId).NotEmpty();
        RuleFor(x => x.Request.OrderId).NotEmpty().MaximumLength(100);
    }
}
