using MediatR;
using Review_Guard.Application.Common.ResultPattern;

namespace Review_Guard.Application.Feature.Auth.Command.VerifyResetCode;

public record VerifyResetCodeCommand(string code) : IRequest<Result<string>>;
