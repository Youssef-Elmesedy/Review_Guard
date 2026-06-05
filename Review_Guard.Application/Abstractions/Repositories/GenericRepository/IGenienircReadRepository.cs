using Review_Guard.Application.Abstractions.Specifications;
using System.Linq.Expressions;

namespace Review_Guard.Application.Abstractions.Repositories.GenericRepository;

/// <summary>
/// Generic read repository abstraction used for querying entities
/// using:
/// - Basic LINQ expressions
/// - Specification pattern
/// - Projection queries (DTO mapping)
/// </summary>
/// <typeparam name="TEntity">
/// Entity type
/// </typeparam>
public interface IGenericReadRepository<TEntity>
    where TEntity : class
{
    // =========================================================
    // BASIC QUERIES
    // =========================================================

    /// <summary>
    /// Returns all entities.
    /// </summary>
    Task<IReadOnlyList<TEntity>> GetAllAsync(
        CancellationToken ct = default);

    /// <summary>
    /// Returns entity by id.
    /// </summary>
    Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default);

    /// <summary>
    /// Returns entities matching predicate.
    /// </summary>
    Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct = default);

    /// <summary>
    /// Returns first entity matching predicate.
    /// </summary>
    Task<TEntity?> FindFirstAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct = default);

    /// <summary>
    /// Checks if any entity matches predicate.
    /// </summary>
    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct = default);

    /// <summary>
    /// Returns total count using optional predicate.
    /// </summary>
    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken ct = default);

    // =========================================================
    // SPECIFICATION QUERIES
    // =========================================================

    /// <summary>
    /// Returns entities using specification.
    /// </summary>
    Task<IReadOnlyList<TEntity>> ListAsync(
        ISpecification<TEntity> specification,
        CancellationToken ct = default);

    /// <summary>
    /// Returns first entity matching specification.
    /// </summary>
    Task<TEntity?> FirstOrDefaultAsync(
        ISpecification<TEntity> specification,
        CancellationToken ct = default);

    /// <summary>
    /// Returns total count using specification.
    /// </summary>
    Task<int> CountAsync(
        ISpecification<TEntity> specification,
        CancellationToken ct = default);

    /// <summary>
    /// Checks if any entity matches specification.
    /// </summary>
    Task<bool> AnyAsync(
        ISpecification<TEntity> specification,
        CancellationToken ct = default);

    // =========================================================
    // PROJECTION QUERIES
    // =========================================================

    /// <summary>
    /// Projects entities into custom DTO/result type.
    /// </summary>
    Task<IReadOnlyList<TResult>> ProjectAsync<TResult>(
        ISpecification<TEntity> specification,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken ct = default);

    /// <summary>
    /// Projects first entity matching specification
    /// into custom DTO/result type.
    /// </summary>
    Task<TResult?> ProjectFirstOrDefaultAsync<TResult>(
        ISpecification<TEntity> specification,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken ct = default);
}