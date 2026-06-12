using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ProofModul.Dto;

namespace Review_Guard.Application.Feature.ProofModul.Services;

public interface IReadProofService
{
    Task<Result<ProofResponseDto>> GetByIdAsync(Guid proofId, CancellationToken ct = default);
    Task<Result<PagedResult<ProofListItemDto>>> GetMyProofsAsync(Guid userId, PaginationParams paging, CancellationToken ct = default);
    Task<Result<PagedResult<ProofListItemDto>>> GetPendingProofsAsync(PaginationParams paging, CancellationToken ct = default);
}
