using MediatR;
using Review_Guard.Application.Abstractions.Storage;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.UserModul.UserService;

namespace Review_Guard.Application.Feature.UserModul.Command.UpdateImage;

internal sealed class UpdateImageCommandHandler : IRequestHandler<UpdateImageCommand, Result<FileUploadResult?>>
{
    private readonly IWriteUserService _writeUserService;
    public UpdateImageCommandHandler(IWriteUserService writeUserService)
    {
        _writeUserService = writeUserService;
    }
    public async Task<Result<FileUploadResult?>> Handle(UpdateImageCommand request, CancellationToken cancellationToken)
    {
        return await _writeUserService.UpdateProfileImage(request.userId, request.fileImage, cancellationToken);
    }
}
