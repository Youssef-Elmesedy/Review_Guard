using MediatR;
using Review_Guard.Application.Abstractions.Repositories.MediaRepository;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.MediaModule.DTOs;

namespace Review_Guard.Application.Feature.MediaModule.Queries.GetMediaByOwner;

internal sealed class GetMediaByOwnerQueryHandler
    : IRequestHandler<GetMediaByOwnerQuery, Result<IReadOnlyList<MediaAssetResponseDto>>>
{
    private readonly IReadMediaRepository _readRepo;

    public GetMediaByOwnerQueryHandler(IReadMediaRepository readRepo)
    {
        _readRepo = readRepo;
    }

    public async Task<Result<IReadOnlyList<MediaAssetResponseDto>>> Handle(
        GetMediaByOwnerQuery request,
        CancellationToken    cancellationToken)
    {
        var assets = await _readRepo.GetByOwnerAsync(
            request.OwnerId,
            request.OwnerType,
            cancellationToken);

        var dtos = assets
            .Select(m => new MediaAssetResponseDto(
                m.Id,
                m.Url,
                m.IsPrimary,
                m.SortOrder,
                m.OwnerType,
                m.CreatedAt))
            .ToList();

        return Result<IReadOnlyList<MediaAssetResponseDto>>.Success(dtos);
    }
}
