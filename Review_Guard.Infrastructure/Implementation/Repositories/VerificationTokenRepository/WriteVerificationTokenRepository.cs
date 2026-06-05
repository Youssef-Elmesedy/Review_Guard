using Review_Guard.Application.Abstractions.Repositories.VerificationTokens;

namespace Review_Guard.Infrastructure.Implementation.Repositories.VerificationTokenRepository;

internal sealed class WriteVerificationTokenRepository : GenericWriteRepository<VerificationCode>, IWriteVerificationTokenRepository
{
    public WriteVerificationTokenRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

}