using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Feature.Auth.Command.ResendVerificationCode;

public record ResendVerificationCodeCommand(string Email, string password, VerificationCodeType Type) : IRequest<Result>;

