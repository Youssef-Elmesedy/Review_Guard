using Review_Guard.Domain.Enums;
using Review_Guard.Domain.ValueObject;

namespace Review_Guard.Application.Common;

/// <summary>
/// Centralized catalog of typed AppError instances.
/// Usage: AppErrorCodes.Media.NotFound, AppErrorCodes.UserError.NotFound, etc.
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
}
