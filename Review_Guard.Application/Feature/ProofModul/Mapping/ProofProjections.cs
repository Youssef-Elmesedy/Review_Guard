using Review_Guard.Application.Feature.ProofModul.Dto;
using Review_Guard.Domain.Entities;
using System.Linq.Expressions;

namespace Review_Guard.Application.Feature.ProofModul.Mapping;

public static class ProofProjections
{
    public static Expression<Func<Proof, ProofResponseDto>> Full =>
        p => new ProofResponseDto(
            p.Id, p.UserId, p.User.FullName, p.BranchId, p.Branch.Address,
            p.Type, p.Status, p.FileUrl, p.OrderId, p.AdminNote, p.CreatedAt);

    public static Expression<Func<Proof, ProofListItemDto>> ListItem =>
        p => new ProofListItemDto(p.Id, p.User.FullName, p.Branch.Address, p.Type, p.Status, p.CreatedAt);
}
