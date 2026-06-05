using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.Auth.DTOs.Responses;

namespace Review_Guard.Application.Feature.Auth.Queries.RefreshToken;

public record RefreshTokenQuery() : IRequest<Result<AuthResponseDto>>;

