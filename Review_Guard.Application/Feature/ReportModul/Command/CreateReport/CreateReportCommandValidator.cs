using FluentValidation;
namespace Review_Guard.Application.Feature.ReportModul.Command.CreateReport;
public sealed class CreateReportCommandValidator : AbstractValidator<CreateReportCommand>
{
    public CreateReportCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Request.ReviewId).NotEmpty();
        RuleFor(x => x.Request.Description).NotEmpty().MinimumLength(10).MaximumLength(1000);
    }
}
