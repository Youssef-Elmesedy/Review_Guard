using Microsoft.Extensions.Localization;
using Review_Guard.Application.Abstractions.Services.CurrentUserService;
using Review_Guard.Application.Common;
using Review_Guard.Application.Common.CommonMessages;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.Auth;
using Review_Guard.Application.Feature.BusinessModul;
using Review_Guard.Application.Feature.BusinessModul.Dto;
using Review_Guard.Application.Feature.BusinessModul.Services;
using Review_Guard.Domain.Exceptions;
using Review_Guard.Domain.Rules;

namespace Review_Guard.Infrastructure.Implementation.Servcices.BusinessService;

internal sealed class WriteBusinessService : IWriteBusinessService
{
    private readonly IReadBusinessRepository _readBusinessRepository;
    private readonly IWriteBusinessRepository _writeBusinessRepository;
    private readonly IReadUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly ICacheService _cache;
    private readonly IStringLocalizer<WriteBusinessService> _stringLocalizer;
    private readonly ILogger<WriteBusinessService> _logger;

    public WriteBusinessService(IReadBusinessRepository readBusinessRepository, IWriteBusinessRepository writeBusinessRepository, IReadUserRepository userRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUser, ICacheService cache, IStringLocalizer<WriteBusinessService> str, ILogger<WriteBusinessService> logger)
    {
        _readBusinessRepository = readBusinessRepository;
        _writeBusinessRepository = writeBusinessRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _cache = cache;
        _stringLocalizer = str;
        _logger = logger;
    }

    public async Task<Result<CreateBusinessResponse>> CreateBusinessAsync(CreateBusinessResponse command, CancellationToken ct = default)
    {
        try
        {
            if (!_currentUser.UserId.HasValue)
                return Result<CreateBusinessResponse>.Failure(AppErrorsCataloge.
                    Unauthorized(_stringLocalizer[CommonMessage.Unauthorized],
                                      _stringLocalizer[CommonMessage.Unauthorized]));

            var userId = _currentUser.UserId.Value;

            // ── Load & validate owner ──────────────────────────────────────────
            var user = await _userRepository.GetByIdAsync(userId, ct);
            if (user is null)
                return Result<CreateBusinessResponse>.Failure(AppErrorsCataloge.
                    NotFound(_stringLocalizer[AuthMessage.UserNotFound],
                            _stringLocalizer[AuthMessage.UserNotFound]));

            UserBusinessRules.AccountMustBeActive(user);

            var existName = await _readBusinessRepository.AnyAsync(b => b.Name == command.Name && b.OwnerId == userId, ct);
            if (existName)
                return Result<CreateBusinessResponse>.Failure(AppErrorsCataloge.
                    Conflict(_stringLocalizer[BusinessMessage.BusinessAlreadyExists],
                             _stringLocalizer[BusinessMessage.BusinessAlreadyExists]));

            var nameExists = await _readBusinessRepository.NameExistsForOwnerAsync(userId, command.Name, ct);
            if (nameExists)
                return Result<CreateBusinessResponse>.Failure(AppErrorsCataloge.
                    Conflict(_stringLocalizer[BusinessMessage.NameExistsForOwnerAsync],
                             _stringLocalizer[BusinessMessage.NameExistsForOwnerAsync]));

            var existcategory = await _readBusinessRepository.AnyAsync(c => c.BusinessCategoryId == command.BusinessCategoryId);
            if (!existcategory)
                return Result<CreateBusinessResponse>.Failure(AppErrorsCataloge.
                    NotFound(_stringLocalizer[BusinessMessage.BusinessCategoryNotFound],
                            _stringLocalizer[BusinessMessage.BusinessCategoryNotFound]));

            // ── Create business ───────────────────────────────────────────────
            var business = Business.Create(
                userId,
                command.Name,
                command.Description,
                command.BusinessCategoryId);

            await _writeBusinessRepository.AddAsync(business, ct);

            await _unitOfWork.SaveChangesAsync(ct);

            var response = new CreateBusinessResponse
            (
                business.Id,
                business.Name,
                business.Description,
                business.OwnerId,
                business.BusinessCategoryId,
                business.Status.ToString()
            );

            await _cache.RemoveByPrefixAsync("business:", ct);

            return Result<CreateBusinessResponse>.Success(response);
        }
        catch (DomainException ex)
        {
            _logger.LogError(ex, "Domain error occurred while creating business. ErrorCode: {ErrorCode}, MessageKey: {MessageKey}", ex.ErrorCode, ex.MessageKey);
            return Result<CreateBusinessResponse>.Failure(AppErrorsCataloge.Failure
                (
                  _stringLocalizer[ex.ErrorCode],
                _stringLocalizer[ex.MessageKey]
                ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while creating business.");
            return Result<CreateBusinessResponse>.Failure(AppErrorsCataloge.Failure
                (
                  _stringLocalizer[BusinessMessage.CreateBusinessFailed],
                _stringLocalizer[BusinessMessage.CreateBusinessFailed]
                ));
        }

    }

    public Task<Result<bool>> DeleteBusinessAsync(Guid businessId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<UpdateBusinessResponse>> UpdateBusinessAsync(UpdateBusinessResponse command, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
