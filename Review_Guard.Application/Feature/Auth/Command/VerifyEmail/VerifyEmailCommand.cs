using MediatR;
using Review_Guard.Application.Common.ResultPattern;

namespace Review_Guard.Application.Feature.Auth.Command.VerifyEmail;

public record VerifyEmailCommand(string Code) : IRequest<Result>;

