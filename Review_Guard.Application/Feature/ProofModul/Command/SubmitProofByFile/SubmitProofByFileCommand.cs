using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ProofModul.Dto;
namespace Review_Guard.Application.Feature.ProofModul.Command.SubmitProofByFile;
public sealed record SubmitProofByFileCommand(Guid UserId, SubmitProofByFileRequest Request) : IRequest<Result<ProofResponseDto>>;
