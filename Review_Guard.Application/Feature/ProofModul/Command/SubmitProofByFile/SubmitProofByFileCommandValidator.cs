using FluentValidation;
namespace Review_Guard.Application.Feature.ProofModul.Command.SubmitProofByFile;
public sealed class SubmitProofByFileCommandValidator : AbstractValidator<SubmitProofByFileCommand>
{
    public SubmitProofByFileCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Request.BranchId).NotEmpty();
        RuleFor(x => x.Request.FileUrl).NotEmpty().MaximumLength(500);
    }
}
