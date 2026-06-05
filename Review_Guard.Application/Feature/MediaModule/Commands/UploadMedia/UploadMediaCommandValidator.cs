using FluentValidation;
using Review_Guard.Application.Feature.MediaModule.Commands.UploadMedia;

namespace Review_Guard.Application.Feature.MediaModule.Commands.UploadMedia;

public sealed class UploadMediaCommandValidator : AbstractValidator<UploadMediaCommand>
{
    private static readonly string[] AllowedTypes =
        { "image/jpeg", "image/jpg", "image/png", "image/webp", "application/pdf" };

    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

    public UploadMediaCommandValidator()
    {
        RuleFor(x => x.OwnerId)
            .NotEmpty()
            .WithMessage("OwnerId is required.");

        RuleFor(x => x.Files)
            .NotNull()
            .NotEmpty()
            .WithMessage("At least one file must be provided.");

        RuleForEach(x => x.Files).ChildRules(file =>
        {
            file.RuleFor(f => f.Length)
                .LessThanOrEqualTo(MaxFileSizeBytes)
                .WithMessage($"Each file must be ≤ {MaxFileSizeBytes / 1024 / 1024} MB.");

            file.RuleFor(f => f.ContentType)
                .Must(ct => AllowedTypes.Contains(ct.ToLowerInvariant()))
                .WithMessage("Allowed file types: jpg, png, webp, pdf.");
        });
    }
}
