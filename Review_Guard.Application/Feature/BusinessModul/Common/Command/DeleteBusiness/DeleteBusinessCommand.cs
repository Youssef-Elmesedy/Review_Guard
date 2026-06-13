using MediatR;
using Review_Guard.Application.Common.ResultPattern;

namespace Review_Guard.Application.Feature.BusinessModul.Common.Command.DeleteBusiness;

public sealed record DeleteBusinessCommand(Guid BusinessId) : IRequest<Result<bool>>;
