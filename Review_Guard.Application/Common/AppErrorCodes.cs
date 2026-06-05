using Review_Guard.Domain.ValueObject;
using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Common;

/// <summary>
/// Centralized catalog of typed AppError instances.
/// Usage: AppErrorCodes.Media.NotFound, AppErrorCodes.User.NotFound, etc.
/// </summary>
public static class AppErrorCodes
{
    public static class Media
    {
        public static readonly AppError NotFound =
            new("Media.NotFound", "Media asset not found.", ErrorType.NotFound);

        public static readonly AppError NoFilesProvided =
            new("Media.NoFiles", "At least one file must be provided.", ErrorType.Validation);

        public static readonly AppError ProfileImageSingleOnly =
            new("Media.ProfileSingleOnly", "Profile image must be a single file.", ErrorType.Validation);

        public static readonly AppError ReorderCountMismatch =
            new("Media.ReorderCountMismatch", "Reorder list must include all media IDs.", ErrorType.Validation);

        public static readonly AppError ReorderInvalidId =
            new("Media.ReorderInvalidId", "One or more media IDs are invalid.", ErrorType.Validation);

        public static readonly AppError Unexpected =
            new("Media.Unexpected", "An unexpected error occurred during upload.", ErrorType.Failure);

        public static AppError TooManyFiles(int max) =>
            new("Media.TooManyFiles", $"You can upload at most {max} files at once.", ErrorType.Validation);

        public static AppError MaxFilesExceeded(int max) =>
            new("Media.MaxFilesExceeded", $"Owner cannot have more than {max} media files.", ErrorType.Validation);

        public static AppError InvalidContentType(string type) =>
            new("Media.InvalidContentType", $"Content type '{type}' is not allowed.", ErrorType.Validation);

        public static AppError UploadFailed(string fileName) =>
            new("Media.UploadFailed", $"Failed to upload file '{fileName}'.", ErrorType.Failure);
    }

    public static class User
    {
        public static readonly AppError NotFound =
            new("User.NotFound", "User not found.", ErrorType.NotFound);

        public static readonly AppError Unauthorized =
            new("User.Unauthorized", "User not authenticated.", ErrorType.Unauthorized);
    }

    public static class Business
    {
        public static readonly AppError NotFound =
            new("Business.NotFound", "Business not found.", ErrorType.NotFound);
    }

    public static class Branch
    {
        public static readonly AppError NotFound =
            new("Branch.NotFound", "Branch not found.", ErrorType.NotFound);
    }

    public static class Review
    {
        public static readonly AppError NotFound =
            new("Review.NotFound", "Review not found.", ErrorType.NotFound);

        public static readonly AppError AlreadyExists =
            new("Review.AlreadyExists", "You have already reviewed this branch.", ErrorType.Conflict);
    }

    public static class Report
    {
        public static readonly AppError NotFound =
            new("Report.NotFound", "Report not found.", ErrorType.NotFound);
    }
}
