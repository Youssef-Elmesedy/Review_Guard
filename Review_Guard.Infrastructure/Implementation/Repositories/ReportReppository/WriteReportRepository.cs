using Review_Guard.Application.Abstractions.Repositories.ReportReppository;
using Review_Guard.Infrastructure.Implementation.Repositories.GeneircRepository;

namespace Review_Guard.Infrastructure.Implementation.Repositories.ReportReppository;

internal sealed class WriteReportRepository : GenericWriteRepository<Report>, IWriteReportRepository
{
    public WriteReportRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}
