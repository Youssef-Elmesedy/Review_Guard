using FluentValidation;
namespace Review_Guard.Application.Feature.ReviewModul.Command.SubmitReview;

public sealed class SubmitReviewCommandValidator : AbstractValidator<SubmitReviewCommand>
{
    public SubmitReviewCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Request.businessId).NotEmpty();
        RuleFor(x => x.Request.BranchId).NotEmpty();
        RuleFor(x => x.Request.Content).NotEmpty().MinimumLength(20).MaximumLength(2000);
        RuleFor(x => x.Request.FoodRating).InclusiveBetween(1, 5);
        RuleFor(x => x.Request.ServiceRating).InclusiveBetween(1, 5);
        RuleFor(x => x.Request.CleanlinessRating).InclusiveBetween(1, 5);
        RuleFor(x => x.Request.AmbienceRating).InclusiveBetween(1, 5);
        RuleFor(x => x.Request.ValueRating).InclusiveBetween(1, 5);
        RuleFor(x => x.Request.ProofId).NotEmpty();
    }
}
