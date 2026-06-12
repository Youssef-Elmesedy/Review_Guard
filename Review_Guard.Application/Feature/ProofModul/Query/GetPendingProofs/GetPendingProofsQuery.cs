using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ProofModul.Dto;
namespace Review_Guard.Application.Feature.ProofModul.Query.GetPendingProofs;
public sealed record GetPendingProofsQuery(int PageNumber, int PageSize) : IRequest<Result<PagedResult<ProofListItemDto>>>;
