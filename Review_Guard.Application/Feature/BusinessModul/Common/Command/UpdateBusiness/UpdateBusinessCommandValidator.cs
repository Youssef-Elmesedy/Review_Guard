using FluentValidation;

namespace Review_Guard.Application.Feature.BusinessModul.Common.Command.UpdateBusiness;

public sealed class UpdateBusinessCommandValidator : AbstractValidator<UpdateBusinessCommand>
{
    public UpdateBusinessCommandValidator()
    {
        RuleFor(x => x.Response.Id).NotEmpty();

        When(x => x.Response.Name is not null, () =>
            RuleFor(x => x.Response.Name).MaximumLength(200));

        When(x => x.Response.Description is not null, () =>
            RuleFor(x => x.Response.Description).MaximumLength(2000));
    }
}
