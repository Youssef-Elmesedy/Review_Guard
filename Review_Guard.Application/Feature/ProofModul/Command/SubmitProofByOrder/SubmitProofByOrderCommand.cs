using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ProofModul.Dto;
namespace Review_Guard.Application.Feature.ProofModul.Command.SubmitProofByOrder;
public sealed record SubmitProofByOrderCommand(Guid UserId, SubmitProofByOrderRequest Request) : IRequest<Result<ProofResponseDto>>;
