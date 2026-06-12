using Review_Guard.Application.Abstractions.Specifications;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Domain.Entities;
using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Feature.ReviewModul.Specification;

public sealed class ReviewByIdSpecification : BaseSpecification<Review>
{
    public ReviewByIdSpecification(Guid reviewId)
    {
        AddCriteria(r => r.Id == reviewId);
        AddInclude(r => r.User);
        AddInclude(r => r.Branch);
    }
}

public sealed class MyReviewsSpecification : BaseSpecification<Review>
{
    public MyReviewsSpecification(Guid userId, PaginationParams paging)
    {
        AddCriteria(r => r.UserId == userId);
        AddInclude(r => r.Branch);
        AddInclude(r => r.User);
        ApplyOrderByDescending(r => r.CreatedAt);
        ApplyPaging(paging.Skip, paging.PageSize);
    }
}

public sealed class PendingReviewsSpecification : BaseSpecification<Review>
{
    public PendingReviewsSpecification(PaginationParams paging)
    {
        AddCriteria(r => r.Status == ReviewStatus.Pending);
        AddInclude(r => r.User);
        AddInclude(r => r.Branch);
        ApplyOrderByDescending(r => r.CreatedAt);
        ApplyPaging(paging.Skip, paging.PageSize);
    }
}
