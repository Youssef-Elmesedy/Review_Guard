using MediatR;
using Review_Guard.Application.Common.ResultPattern;

namespace Review_Guard.Application.Feature.BusinessModul.Common.Command.RejectBusiness;

public sealed record RejectBusinessCommand(Guid BusinessId, string Reason) : IRequest<Result>;
