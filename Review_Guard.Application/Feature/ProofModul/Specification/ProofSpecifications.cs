using Review_Guard.Application.Abstractions.Specifications;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Domain.Entities;
using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Feature.ProofModul.Specification;

public sealed class ProofByIdSpecification : BaseSpecification<Proof>
{
    public ProofByIdSpecification(Guid proofId)
    {
        AddCriteria(p => p.Id == proofId);
        AddInclude(p => p.User);
        AddInclude(p => p.Branch);
    }
}

public sealed class MyProofsSpecification : BaseSpecification<Proof>
{
    public MyProofsSpecification(Guid userId, PaginationParams paging)
    {
        AddCriteria(p => p.UserId == userId);
        AddInclude(p => p.Branch);
        AddInclude(p => p.User);
        ApplyOrderByDescending(p => p.CreatedAt);
        ApplyPaging(paging.Skip, paging.PageSize);
    }
}

public sealed class PendingProofsSpecification : BaseSpecification<Proof>
{
    public PendingProofsSpecification(PaginationParams paging)
    {
        AddCriteria(p => p.Status == ProofStatus.Pending);
        AddInclude(p => p.User);
        AddInclude(p => p.Branch);
        ApplyOrderByDescending(p => p.CreatedAt);
        ApplyPaging(paging.Skip, paging.PageSize);
    }
}
