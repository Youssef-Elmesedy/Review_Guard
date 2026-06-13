using Review_Guard.Application.Abstractions.Repositories.BusinessCategoryRepository;
using Review_Guard.Infrastructure.Implementation.Repositories.GeneircRepository;

namespace Review_Guard.Infrastructure.Implementation.Repositories.BusinessCategoryRepository;

internal sealed class ReadBusinessCategoryRepository
    : GenericReadRepository<BusinessCategory>, IReadBusinessCategoryRepository
{
    public ReadBusinessCategoryRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}
