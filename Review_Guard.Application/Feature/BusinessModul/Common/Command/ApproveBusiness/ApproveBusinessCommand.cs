using MediatR;
using Review_Guard.Application.Common.ResultPattern;

namespace Review_Guard.Application.Feature.BusinessModul.Common.Command.ApproveBusiness;

public sealed record ApproveBusinessCommand(Guid BusinessId, string? Note) : IRequest<Result>;
