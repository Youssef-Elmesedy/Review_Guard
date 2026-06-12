using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ProofModul.Dto;
namespace Review_Guard.Application.Feature.ProofModul.Command.RejectProof;
public sealed record RejectProofCommand(Guid AdminId, Guid ProofId, AdminProofActionRequest Request) : IRequest<Result>;
