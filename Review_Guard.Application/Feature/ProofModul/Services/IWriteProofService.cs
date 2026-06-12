using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ProofModul.Dto;

namespace Review_Guard.Application.Feature.ProofModul.Services;

public interface IWriteProofService
{
    Task<Result<ProofResponseDto>> SubmitByFileAsync(Guid userId, SubmitProofByFileRequest request, CancellationToken ct = default);
    Task<Result<ProofResponseDto>> SubmitByOrderAsync(Guid userId, SubmitProofByOrderRequest request, CancellationToken ct = default);
    Task<Result> VerifyAsync(Guid adminId, Guid proofId, AdminProofActionRequest request, CancellationToken ct = default);
    Task<Result> RejectAsync(Guid adminId, Guid proofId, AdminProofActionRequest request, CancellationToken ct = default);
}
