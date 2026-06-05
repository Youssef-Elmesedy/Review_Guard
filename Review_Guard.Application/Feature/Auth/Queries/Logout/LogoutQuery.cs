using MediatR;
using Review_Guard.Application.Common.ResultPattern;

namespace Review_Guard.Application.Feature.Auth.Queries.Logout;

public record LogoutQuery() : IRequest<Result<string>>;
