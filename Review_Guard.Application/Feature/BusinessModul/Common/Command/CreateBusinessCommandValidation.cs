using FluentValidation;

namespace Review_Guard.Application.Feature.BusinessModul.Common.Command;

public class CreateBusinessCommandValidation : AbstractValidator<CreateBusinessCommand>
{
    public CreateBusinessCommandValidation()
    {
        RuleFor(c => c.Response.Name)
            .NotEmpty()
            .WithMessage("Can't set Name for Empty.");

        RuleFor(c => c.Response.Description)
            .NotEmpty()
            .WithMessage("Can't set Descripition for Empty.");

        RuleFor(c => c.Response.OwnerId)
            .NotEmpty()
            .WithMessage("Can't set OwnerId for Empty");

        RuleFor(c => c.Response.BusinessCategoryId)
            .NotEmpty()
            .WithMessage("Can't set Category Id for Empty");

    }
}
