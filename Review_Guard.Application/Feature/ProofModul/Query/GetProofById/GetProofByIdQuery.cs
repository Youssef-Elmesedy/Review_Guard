using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ProofModul.Dto;
namespace Review_Guard.Application.Feature.ProofModul.Query.GetProofById;
public sealed record GetProofByIdQuery(Guid ProofId) : IRequest<Result<ProofResponseDto>>;
