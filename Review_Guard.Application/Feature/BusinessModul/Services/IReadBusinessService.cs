using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.BusinessModul.Dto;

namespace Review_Guard.Application.Feature.BusinessModul.Services;

public interface IReadBusinessService
{
    Task<Result<PagedResult<BusinessListtDto>>> GetAllBusiness(PaginationParams paginationParams, CancellationToken ct = default);
    Task<Result<BusinessListWithBranchsDto>> GetByBusinessIdWithBranchs(Guid businessId, CancellationToken ct = default);
    Task<Result<PagedResult<BusinessListWithBranchsDto>>> GetAllBusinessWithBranchsByCategoryAsync(Guid categoryId, PaginationParams paginationParams, CancellationToken ct = default);
    Task<Result<PagedResult<BusinessWithReviewDto>>> GetAllBusinessWithReview(PaginationParams paginationParams, CancellationToken ct = default);

    Task<Result<BranchWithReviewsDto>> GetByBranchIdWithReview(Guid branchId, CancellationToken ct = default);

    Task<Result<PagedResult<BusinessListWithBranchsDto>>> SearchByBusinessName(string businessName, PaginationParams paginationParams, CancellationToken ct = default);
}
