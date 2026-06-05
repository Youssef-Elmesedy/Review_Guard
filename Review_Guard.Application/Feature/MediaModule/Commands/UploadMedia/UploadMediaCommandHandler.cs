using MediatR;
using Review_Guard.Application.Abstractions.Services.MediaService;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.MediaModule.DTOs;

namespace Review_Guard.Application.Feature.MediaModule.Commands.UploadMedia;

internal sealed class UploadMediaCommandHandler
    : IRequestHandler<UploadMediaCommand, Result<IReadOnlyList<MediaUploadResponseDto>>>
{
    private readonly IMediaService _mediaService;

    public UploadMediaCommandHandler(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    public async Task<Result<IReadOnlyList<MediaUploadResponseDto>>> Handle(
        UploadMediaCommand request,
        CancellationToken  cancellationToken)
    {
        var result = await _mediaService.UploadAsync(
            request.OwnerId,
            request.OwnerType,
            request.Files,
            cancellationToken);

        if (result.IsFailure)
            return Result<IReadOnlyList<MediaUploadResponseDto>>.Failure(result.Error!);

        // Map IMediaService DTOs → Feature DTOs (keeps layers decoupled)
        var mapped = result.Value
            .Select(x => new MediaUploadResponseDto(x.Id, x.Url, x.IsPrimary, x.SortOrder))
            .ToList();

        return Result<IReadOnlyList<MediaUploadResponseDto>>.Success(mapped);
    }
}
