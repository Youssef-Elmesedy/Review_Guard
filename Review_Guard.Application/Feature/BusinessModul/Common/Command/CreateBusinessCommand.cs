using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.BusinessModul.Dto;

namespace Review_Guard.Application.Feature.BusinessModul.Common.Command;

public record CreateBusinessCommand(CreateBusinessResponse Response) : IRequest<Result<CreateBusinessResponse>>;
