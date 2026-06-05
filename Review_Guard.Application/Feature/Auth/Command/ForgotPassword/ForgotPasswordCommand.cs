using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.Auth.DTOs.Requests;
using Review_Guard.Application.Feature.Auth.DTOs.Responses;

namespace Review_Guard.Application.Feature.Auth.Command.ForgotPassword;

public record ForgotPasswordCommand(ForgotPasswordDto ForgotPasswordDto) : IRequest<Result<MessageResponseDto>>;

