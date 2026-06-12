using MediatR;
using Microsoft.AspNetCore.Http;
using Review_Guard.Application.Abstractions.Storage;
using Review_Guard.Application.Common.ResultPattern;

namespace Review_Guard.Application.Feature.UserModul.Command.UpdateImage;

public record UpdateImageCommand(
    Guid userId,
    IFormFile fileImage) : IRequest<Result<FileUploadResult?>>;
