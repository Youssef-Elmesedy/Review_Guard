using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ProofModul.Dto;
namespace Review_Guard.Application.Feature.ProofModul.Command.VerifyProof;
public sealed record VerifyProofCommand(Guid AdminId, Guid ProofId, AdminProofActionRequest Request) : IRequest<Result>;
