using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.BusinessModul.Dto;

namespace Review_Guard.Application.Feature.BusinessModul.Services;

public interface IWriteBusinessService
{
    Task<Result<CreateBusinessResponse>> CreateBusinessAsync(CreateBusinessResponse command, CancellationToken ct = default);
    Task<Result<UpdateBusinessResponse>> UpdateBusinessAsync(UpdateBusinessResponse command, CancellationToken ct = default);
    Task<Result<bool>> DeleteBusinessAsync(Guid businessId, CancellationToken ct = default);

    Task<Result> ApproveBusinessAsync(Guid businessId, string? note, CancellationToken ct = default);
    Task<Result> RejectBusinessAsync(Guid businessId, string reason, CancellationToken ct = default);
}
