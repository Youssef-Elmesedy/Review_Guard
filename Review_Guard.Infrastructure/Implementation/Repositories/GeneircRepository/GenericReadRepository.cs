using Review_Guard.Application.Abstractions.Specifications;
using Review_Guard.Infrastructure.Implementation.Specifications;

namespace Review_Guard.Infrastructure.Implementation.Repositories.GeneircRepository;

/// <summary>
/// Generic read repository for querying entities using:
/// - Basic LINQ predicates
/// - Specifications
/// - Projections
/// </summary>
/// <typeparam name="TEntity">
/// Entity type
/// </typeparam>
internal class GenericReadRepository<TEntity>
    : IGenericReadRepository<TEntity>
    where TEntity : class
{
    protected readonly AppDbContext _appDbContext;
    protected readonly DbSet<TEntity> _dbSet;

    public GenericReadRepository(
        AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;

        _dbSet = _appDbContext.Set<TEntity>();
    }

    // =========================================================
    // BASIC QUERIES
    // =========================================================

    /// <summary>
    /// Checks if any entity matches the predicate.
    /// </summary>
    public async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(predicate, ct);
    }

    /// <summary>
    /// Returns total count with optional predicate.
    /// </summary>
    public async Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken ct = default)
    {
        return predicate is null
            ? await _dbSet
                .AsNoTracking()
                .CountAsync(ct)

            : await _dbSet
                .AsNoTracking()
                .CountAsync(predicate, ct);
    }

    /// <summary>
    /// Returns all entities.
    /// </summary>
    public async Task<IReadOnlyList<TEntity>> GetAllAsync(
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .ToListAsync(ct);
    }

    /// <summary>
    /// Returns entity by id.
    /// </summary>
    public async Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await _dbSet.FindAsync(
            new object[] { id },
            ct);
    }

    /// <summary>
    /// Returns first entity matching predicate.
    /// </summary>
    public async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(predicate, ct);
    }

    /// <summary>
    /// Returns all entities matching predicate.
    /// </summary>
    public async Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Alias for FindAsync.
    /// </summary>
    public async Task<IReadOnlyList<TEntity>> WhereAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Returns first entity matching predicate.
    /// </summary>
    public async Task<TEntity?> FindFirstAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(predicate, ct);
    }

    // =========================================================
    // SPECIFICATION QUERIES
    // =========================================================

    /// <summary>
    /// Checks if any entity matches specification.
    /// </summary>
    public async Task<bool> AnyAsync(
        ISpecification<TEntity> specification,
        CancellationToken ct = default)
    {
        var query = ApplySpecification(specification);

        return await query.AnyAsync(ct);
    }

    /// <summary>
    /// Returns total count using specification.
    /// </summary>
    public async Task<int> CountAsync(
        ISpecification<TEntity> specification,
        CancellationToken ct = default)
    {
        var query = ApplySpecification(specification);

        return await query.CountAsync(ct);
    }

    /// <summary>
    /// Returns first entity matching specification.
    /// </summary>
    public async Task<TEntity?> FirstOrDefaultAsync(
        ISpecification<TEntity> specification,
        CancellationToken ct = default)
    {
        var query = ApplySpecification(specification);

        return await query.FirstOrDefaultAsync(ct);
    }

    /// <summary>
    /// Returns list using specification.
    /// </summary>
    public async Task<IReadOnlyList<TEntity>> ListAsync(
        ISpecification<TEntity> specification,
        CancellationToken ct = default)
    {
        var query = ApplySpecification(specification);

        return await query.ToListAsync(ct);
    }

    // =========================================================
    // PROJECTION QUERIES
    // =========================================================

    /// <summary>
    /// Projects entities into DTO/result type.
    /// </summary>
    public async Task<IReadOnlyList<TResult>> ProjectAsync<TResult>(
        ISpecification<TEntity> specification,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken ct = default)
    {
        var query = ApplySpecification(specification);

        return await query
            .Select(selector)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Projects first entity matching specification
    /// into custom DTO/result type.
    /// </summary>
    public async Task<TResult?> ProjectFirstOrDefaultAsync<TResult>(
        ISpecification<TEntity> specification,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken ct = default)
    {
        var query = ApplySpecification(specification);

        return await query
            .Select(selector)
            .FirstOrDefaultAsync(ct);
    }

    // =========================================================
    // PRIVATE HELPERS
    // =========================================================

    /// <summary>
    /// Applies specification to query.
    /// </summary>
    private IQueryable<TEntity> ApplySpecification(
        ISpecification<TEntity> specification)
    {
        return SpecificationEvaluator<TEntity>.GetQuery(
            _dbSet.AsQueryable(),
            specification);
    }
}