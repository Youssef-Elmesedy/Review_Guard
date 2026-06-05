using Review_Guard.Application.Abstractions.Repositories.VerificationTokens;

namespace Review_Guard.Infrastructure.Implementation.Repositories.VerificationTokenRepository;

internal sealed class ReadVerificationTokenRepository : GenericReadRepository<VerificationCode>, IReadVerificationTokenRepository
{
    public ReadVerificationTokenRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}
