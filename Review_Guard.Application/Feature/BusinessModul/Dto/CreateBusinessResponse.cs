using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Feature.BusinessModul.Dto;

public record CreateBusinessResponse
(
     Guid Id,
     string Name,
     string Description,
     Guid OwnerId,
     Guid BusinessCategoryId,
     string Status
);

public record UpdateBusinessResponse
(
     Guid Id,
     string Name,
     string Description,
     Guid OwnerId,
     Guid BusinessCategoryId,
     BusinessStatus Status
);