using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Feature.ProofModul.Dto;

public sealed record SubmitProofByFileRequest(Guid BranchId, string FileUrl);
public sealed record SubmitProofByOrderRequest(Guid BranchId, string OrderId);
public sealed record AdminProofActionRequest(string? Note);

public sealed record ProofResponseDto(
    Guid        Id,
    Guid        UserId,
    string      UserFullName,
    Guid        BranchId,
    string      BranchAddress,
    ProofType   Type,
    ProofStatus Status,
    string?     FileUrl,
    string?     OrderId,
    string?     AdminNote,
    DateTime    CreatedAt
);

public sealed record ProofListItemDto(
    Guid        Id,
    string      UserFullName,
    string      BranchAddress,
    ProofType   Type,
    ProofStatus Status,
    DateTime    CreatedAt
);
