using MediatR;
using Review_Guard.Application.Abstractions.Services.MediaService;
using Review_Guard.Application.Common.ResultPattern;

namespace Review_Guard.Application.Feature.MediaModule.Commands.ReorderMedia;

internal sealed class ReorderMediaCommandHandler
    : IRequestHandler<ReorderMediaCommand, Result>
{
    private readonly IMediaService _mediaService;

    public ReorderMediaCommandHandler(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    public async Task<Result> Handle(
        ReorderMediaCommand request,
        CancellationToken   cancellationToken)
    {
        return await _mediaService.ReorderAsync(
            request.OwnerId,
            request.OwnerType,
            request.OrderedIds,
            cancellationToken);
    }
}
