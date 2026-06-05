using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.Auth.DTOs.Requests;
using Review_Guard.Application.Feature.Auth.DTOs.Responses;

namespace Review_Guard.Application.Feature.Auth.Command.Login.Admin;

public record LoginAdminCommand(LoginDto LoginDto) : IRequest<Result<AuthResponseDto>>;
