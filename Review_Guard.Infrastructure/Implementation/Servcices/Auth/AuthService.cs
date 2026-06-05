using Microsoft.Extensions.Localization;
using Review_Guard.Application.Abstractions.Services.CurrentUserService;
using Review_Guard.Application.Common;
using Review_Guard.Application.Common.CommonMessages;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.Auth;
using Review_Guard.Application.Feature.Auth.DTOs.Requests;
using Review_Guard.Application.Feature.Auth.DTOs.Responses;
using Review_Guard.Domain.Enums;
using Review_Guard.Domain.Exceptions;
using System.Security;

namespace Review_Guard.Infrastructure.Implementation.Servcices.Auth;

internal sealed class AuthService : IAuthService
{
    private readonly IReadAdminRepository _readAdmin;
    private readonly IWriteAdminRepository _writeAdmin;

    private readonly IReadUserRepository _readUser;
    private readonly IWriteUserRepository _writeUser;

    private readonly IReadVerificationTokenRepository _readVerificationCode;
    private readonly IWriteVerificationTokenRepository _WrietVerifiedToken;

    private readonly IReadUserActivityRepository _readUserActivity;
    private readonly IWriteUserActivityRepository _writeUserActivity;

    private readonly IVerificationCodeService _verificationTokenService;

    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ICurrentUserService _currentUser;
    private readonly IEmailService _emailService;
    private readonly IGeoLocationService _geoLocationService;

    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AuthService> _logger;
    private readonly IStringLocalizer<AuthService> _stringLocalizer;

    public AuthService(IReadAdminRepository readAdmin, IWriteAdminRepository writeAdmin, IReadUserRepository readUser, IWriteUserRepository writeUser, IReadVerificationTokenRepository readVerificationToken, IWriteVerificationTokenRepository wrietVerifiedToken, IReadUserActivityRepository readUserActivity, IWriteUserActivityRepository writeUserActivity, IJwtService jwtService, IPasswordHasher passwordHasher, IRefreshTokenService refreshTokenService, ICurrentUserService currentUser, IEmailService emailService, IUnitOfWork unitOfWork, ILogger<AuthService> logger, IStringLocalizer<AuthService> stringLocalizer, IVerificationCodeService verificationTokenService, IGeoLocationService geoLocationService)
    {
        _readAdmin = readAdmin;
        _writeAdmin = writeAdmin;
        _readUser = readUser;
        _writeUser = writeUser;
        _readVerificationCode = readVerificationToken;
        _WrietVerifiedToken = wrietVerifiedToken;
        _readUserActivity = readUserActivity;
        _writeUserActivity = writeUserActivity;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
        _refreshTokenService = refreshTokenService;
        _currentUser = currentUser;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _stringLocalizer = stringLocalizer;
        _verificationTokenService = verificationTokenService;
        _geoLocationService = geoLocationService;
    }


    // ─────────────────────────────────────────────────────────
    // Register User
    // ─────────────────────────────────────────────────────────
    public async Task<Result<AuthResponseDto>> RegisterUserAsync(
    RegisterUserDto request,
    CancellationToken ct)
    {
        var exists = await _readUser.AnyAsync(x => x.Email == request.Email, ct);

        if (exists)
            return Result<AuthResponseDto>.Failure(AppErrorsCataloge.Conflict(
                "A user with the given email already exists.",
                _stringLocalizer[AuthMessage.UserAlreadyExists]));

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = User.Create(request.FullName, request.Email, passwordHash);

        var VeificationCode = VerificationCode.Create(
            user.Id,
            VerificationCodeType.EmailVerification,
            15);

        var location = await _geoLocationService
            .GetLocationAsync(_currentUser.IpAddress, ct);

        var userActivity = await LogUserActivityAsync(
                            user.Id,
                            _currentUser.AdminId,
                            ActivityType.Register,
                            "Registered User",
                            ct);

        user.RaiseRegisteredEvent(VeificationCode.Code);

        await _unitOfWork.ExecuteAsync(async () =>
        {
            await _writeUser.AddAsync(user, ct);
            await _WrietVerifiedToken.AddAsync(VeificationCode, ct);
            await _writeUserActivity.AddAsync(userActivity, ct);

        }, ct);

        var accessToken = _jwtService.GenerateUserToken(user);

        var refreshToken = _refreshTokenService.Generate(
            user.Id,
            null,
            userActivity.IpAddress);

        return Result<AuthResponseDto>.Success(new AuthResponseDto(
            accessToken,
            refreshToken.Token,
            refreshToken.ExpiresAtUtc,
            user.Role.ToString(),
            user.Id,
            user.Email));
    }

    // ─────────────────────────────────────────────────────────
    // Login User
    // ─────────────────────────────────────────────────────────
    public async Task<Result<AuthResponseDto>> LoginUserAsync(
        LoginDto request,
        CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("Login attempt for User: {Email}", request.Email);

            var user = await _readUser.FindFirstAsync(
                u => u.Email == request.Email, ct);

            if (user is null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid credentials for: {Email}", request.Email);

                return Result<AuthResponseDto>.Failure(
                    AppErrorsCataloge.Unauthorized(
                        "Invalid credentials.",
                        _stringLocalizer[AuthMessage.InvalidCredentials]));
            }

            // Will throw DomainException if suspended/banned
            user.EnsureCanLogin();

            var location = await _geoLocationService
                     .GetLocationAsync(_currentUser.IpAddress, ct);

            var userActivity = await LogUserActivityAsync(
                            user.Id,
                            _currentUser.AdminId,
                            ActivityType.Login,
                            "Login User",
                            ct);

            await _unitOfWork.ExecuteAsync(async () =>
            {
                await _writeUserActivity.AddAsync(userActivity, ct);
            }, ct);

            var accessToken = _jwtService.GenerateUserToken(user);

            var refreshToken = await _refreshTokenService.GetOrCreateAsync(
                user.Id,
                adminId: null,
                ipAddress: _currentUser.IpAddress);

            _logger.LogInformation("User {UserId} logged in successfully", user.Id);

            return Result<AuthResponseDto>.Success(new AuthResponseDto(
                accessToken,
                refreshToken.Token,
                refreshToken.ExpiresAtUtc,
                user.Role.ToString(),
                user.Id,
                user.Email));
        }
        catch (DomainException ex)
        {
            _logger.LogWarning("Login blocked for {Email}: {Reason}", request.Email, ex.Message);

            return Result<AuthResponseDto>.Failure(
                AppErrorsCataloge.Validation(
                    ex.Message, _stringLocalizer[ex.MessageKey]));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for admin {Email}", request.Email);

            return Result<AuthResponseDto>.Failure(
                AppErrorsCataloge.Failure(
                    AuthMessage.LoginFailed, _stringLocalizer[AuthMessage.LoginFailed]));
        }
    }

    // ─────────────────────────────────────────────────────────
    // Login Admin
    // ─────────────────────────────────────────────────────────
    public async Task<Result<AuthResponseDto>> LoginAdminAsync(LoginDto request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("Login attempt for Admin: {Email}", request.Email);

            var admin = await _readAdmin.FindFirstAsync(
                u => u.Email == request.Email, ct);

            if (admin is null || !_passwordHasher.VerifyPassword(request.Password, admin.PasswordHash))
            {
                _logger.LogWarning("Invalid credentials for: {Email}", request.Email);

                return Result<AuthResponseDto>.Failure(
                    AppErrorsCataloge.Unauthorized(
                        "Invalid credentials.",
                        _stringLocalizer[AuthMessage.InvalidCredentials]));
            }

            var accessToken = _jwtService.GenerateAdminToken(admin);

            var refreshToken = await _refreshTokenService.GetOrCreateAsync(
                null,
                adminId: admin.Id,
                ipAddress: _currentUser.IpAddress);

            _logger.LogInformation("Admin {admin.Id} logged in successfully", admin.Id);

            var adminActivity = await LogUserActivityAsync(
                            userId: null,
                            admin.Id,
                            ActivityType.Login,
                            "Login Admin",
                            ct);

            await _unitOfWork.ExecuteAsync(async () =>
            {
                await _writeUserActivity.AddAsync(adminActivity, ct);
            }, ct);

            return Result<AuthResponseDto>.Success(new AuthResponseDto(
                accessToken,
                refreshToken.Token,
                refreshToken.ExpiresAtUtc,
                $"Is Super Admin: ({admin.Role})",
                admin.Id,
                admin.Email));
        }
        catch (DomainException ex)
        {
            _logger.LogWarning("Login blocked for {Email}: {Reason}", request.Email, ex.Message);

            return Result<AuthResponseDto>.Failure(
                AppErrorsCataloge.Validation(
                    ex.Message, _stringLocalizer[ex.MessageKey]));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for admin {Email}", request.Email);

            return Result<AuthResponseDto>.Failure(
                AppErrorsCataloge.Failure(
                    AuthMessage.LoginFailed, _stringLocalizer[AuthMessage.LoginFailed]));
        }
    }


    // ─────────────────────────────────────────────────────────
    // Login Logout
    // ─────────────────────────────────────────────────────────
    public async Task<Result<string>> LogoutAsync(CancellationToken ct)
    {
        try
        {
            var tokenValue = _currentUser.RefreshToken;

            if (string.IsNullOrWhiteSpace(tokenValue))
            {
                return Result<string>.Failure(
                    AppErrorsCataloge.Unauthorized(
                        "Invalid refresh token.",
                        _stringLocalizer[AuthMessage.InvalidRefreshToken]));
            }

            var token = await _refreshTokenService.GetAsync(tokenValue, ct);

            if (token is null)
                return Result<string>.Failure(
                    AppErrorsCataloge.Unauthorized(
                        "Invalid refresh token.",
                        _stringLocalizer[AuthMessage.InvalidRefreshToken]));

            try
            {
                await _refreshTokenService.ValidateOwnershipAsync(token, _currentUser.UserId, _currentUser.AdminId);
            }
            catch (SecurityException)
            {
                return Result<string>.Failure(
                    AppErrorsCataloge.Unauthorized(
                        "Invalid refresh token.",
                        _stringLocalizer[AuthMessage.InvalidRefreshToken]));
            }

            if (!token.IsActive)
                return Result<string>.Failure(
                    AppErrorsCataloge.Unauthorized(
                        "Refresh token expired.",
                        _stringLocalizer[AuthMessage.RefreshTokenExpired]));


            var userActivity = LogUserActivityAsync(
                token.UserId,
                token.AdminId,
                ActivityType.Logout,
                _currentUser.IsAdmin ? "Logou Admin" : "Logout User",
                ct);

            await _unitOfWork.ExecuteAsync(async () =>
            {
                await _refreshTokenService.RevokeAsync(token, _currentUser.IpAddress!, "User Logged Out", ct);

                await _writeUserActivity.AddAsync(await userActivity, ct);

                await _unitOfWork.SaveChangesAsync();
            }
            , ct);

            _logger.LogInformation("User {UserId} logged out successfully", _currentUser.UserId);

            return Result<string>.Success(_stringLocalizer[AuthMessage.LogoutSuccessfully]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Logout Filed");
            throw;
        }
    }

    // ─────────────────────────────────────────────────────────
    // Login Refresh Token 
    // ─────────────────────────────────────────────────────────
    public async Task<Result<AuthResponseDto>> RefreshTokenAsync(CancellationToken ct)
    {
        try
        {
            var tokenValue = _currentUser.RefreshToken;

            if (string.IsNullOrWhiteSpace(tokenValue))
            {
                return Result<AuthResponseDto>.Failure(
                    AppErrorsCataloge.Unauthorized(
                        "Invalid refresh token.",
                        _stringLocalizer[AuthMessage.InvalidRefreshToken]));
            }

            var token = await _refreshTokenService.GetAsync(tokenValue, ct);

            if (token is null)
            {
                return Result<AuthResponseDto>.Failure(
                    AppErrorsCataloge.Unauthorized(
                        "Invalid refresh token.",
                        _stringLocalizer[AuthMessage.InvalidRefreshToken]));
            }

            try
            {
                await _refreshTokenService.ValidateOwnershipAsync(token, _currentUser.UserId, _currentUser.AdminId);
            }
            catch (SecurityException)
            {
                return Result<AuthResponseDto>.Failure(
                    AppErrorsCataloge.Unauthorized(
                        "Invalid refresh token.",
                        _stringLocalizer[AuthMessage.InvalidRefreshToken]));
            }

            if (!token.IsActive)
            {
                return Result<AuthResponseDto>.Failure(
                    AppErrorsCataloge.Unauthorized(
                        "Refresh token expired.",
                        _stringLocalizer[AuthMessage.RefreshTokenExpired]));
            }

            User? user = null;
            Admin? admin = null;

            if (token.UserId.HasValue)
            {
                user = await _readUser
                    .GetByIdAsync(token.UserId.Value, ct);
            }

            if (token.AdminId.HasValue)
            {
                admin = await _readAdmin
                    .GetByIdAsync(token.AdminId.Value, ct);
            }

            if (user is null && admin is null)
            {
                return Result<AuthResponseDto>.Failure(
                    AppErrorsCataloge.NotFound(
                        "User or admin not found.",
                        _stringLocalizer[CommonMessage.NotFound]));
            }

            string accessToken;

            if (user is not null)
            {
                accessToken =
                    _jwtService.GenerateUserToken(user);
            }
            else
            {
                accessToken =
                    _jwtService.GenerateAdminToken(admin!);
            }

            var newRefreshToken =
                 await _refreshTokenService.RotateAsync(token, _currentUser.IpAddress!, ct);

            return Result<AuthResponseDto>.Success(
                new AuthResponseDto(
                    accessToken,
                    newRefreshToken.Token,
                    newRefreshToken.ExpiresAtUtc,
                    user?.Role.ToString() ?? "Super Admin",
                    user?.Id ?? admin!.Id,
                    user?.Email ?? admin!.Email));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Refresh token failed");

            return Result<AuthResponseDto>.Failure(
                AppErrorsCataloge.Failure(
                    "Unexpected error occurred.",
                    _stringLocalizer[CommonMessage.UnexpectedError]));
        }
    }

    // ─────────────────────────────────────────────────────────
    // Forgot Password
    // ─────────────────────────────────────────────────────────
    public async Task<Result<MessageResponseDto>> ForgotPasswordAsync(
    ForgotPasswordDto dto,
    CancellationToken ct)
    {
        var user =
            await _readUser.FindFirstAsync(
                x => x.Email == dto.Email,
                ct);

        if (user is null)
        {
            return Result<MessageResponseDto>.Success(new MessageResponseDto(
                _stringLocalizer[AuthMessage.IfTheEmailExistsAResetCodeWasSent]));
        }

        var verificationCode =
            await _verificationTokenService.CreateOrRefreshAsync(
                user.Id,
                VerificationCodeType.PasswordReset,
                15,
                ct);

        await _emailService.SendPasswordResetAsync(
            user.Email,
            user.FullName,
            verificationCode.Code,
            ct);

        return Result<MessageResponseDto>.Success(new MessageResponseDto(
            _stringLocalizer[AuthMessage.ResetPasswordCodeSentSuccessfully]));
    }

    // ─────────────────────────────────────────────────────────
    // Verify Code 
    // ─────────────────────────────────────────────────────────
    public async Task<Result<string>> VerifyResetCodeAsync(
    string code,
    CancellationToken ct)
    {
        try
        {
            var (Code, user) = await ValidateVerificationCodeAsync(
            code,
            VerificationCodeType.PasswordReset,
            ct);


            return Result<string>.Success($"{user.Email}, {Code.Code}");
        }
        catch (DomainException ex)
        {
            return Result<string>.Failure(
                AppErrorsCataloge.Validation(
                    ex.ErrorCode,
                    _stringLocalizer[ex.MessageKey]));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Verification code validation failed");
            throw;
        }
    }

    // ─────────────────────────────────────────────────────────
    // Reset Password
    // ─────────────────────────────────────────────────────────
    public async Task<Result<MessageResponseDto>> ResetPasswordAsync(
    ResetPasswordDto dto,
    CancellationToken ct)
    {
        var (Code, user) = await ValidateVerificationCodeAsync(
            dto.Code,
            VerificationCodeType.PasswordReset,
            ct);


        if (dto.NewPassword != dto.ConfirmPassword)
            return Result<MessageResponseDto>.Failure(
                AppErrorsCataloge.Validation(
                    "Passwords do not match.",
                    _stringLocalizer[AuthMessage.PasswordsDoNotMatch]));

        user.ChangePassword(
            _passwordHasher.HashPassword(dto.NewPassword));

        Code.MarkAsUsed();

        var activity = await LogUserActivityAsync(
            user.Id,
            _currentUser.AdminId,
            ActivityType.PasswordChanged,
            "Password reset successful",
            ct);

        await _unitOfWork.ExecuteAsync(async () =>
        {
            await _writeUser.UpdateAsync(user, ct);
            await _WrietVerifiedToken.UpdateAsync(Code, ct);
            await _writeUserActivity.AddAsync(activity, ct);
        }, ct);

        return Result<MessageResponseDto>.Success(
            new MessageResponseDto(_stringLocalizer[AuthMessage.UpdatePasswordSuccessfully]));
    }
    // ─────────────────────────────────────────────────────────
    // Login Verify Email
    // ─────────────────────────────────────────────────────────
    public async Task VerifyEmailAsync(string code, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("Email verification attempt");

            var (verificationToken, user) = await ValidateVerificationCodeAsync(
               code,
               VerificationCodeType.EmailVerification,
               ct);

            verificationToken.MarkAsUsed();

            user.VerifyEmail(code);

            var userActivity = await LogUserActivityAsync(
                           user.Id,
                           _currentUser.AdminId,
                           ActivityType.EmailVerified,
                           "Email Verified",
                           ct);

            await _unitOfWork.ExecuteAsync(async () =>
            {
                await _WrietVerifiedToken.UpdateAsync(verificationToken, ct);
                await _writeUser.UpdateAsync(user, ct);
                await _writeUserActivity.AddAsync(userActivity, ct);
            }, ct);

            _logger.LogInformation("Email verified for User {UserId}", user.Id);
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email verification failed");
            throw;
        }
    }

    // ─────────────────────────────────────────────────────────
    // Login Resend Verification Token
    // ─────────────────────────────────────────────────────────
    public async Task<Result<bool>> ResendVerificationCodeAsync(string email, string password, VerificationCodeType type, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation($"Resending Verification Code to {email}");
            var user = await _readUser.FindFirstAsync(e => e.Email.Equals(email), ct);

            if (user is null)
                return Result<bool>.Failure(AppErrorsCataloge.NotFound(
                    "User not found.",
                    _stringLocalizer[AuthMessage.UserNotFound]));

            if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
                return Result<bool>.Failure(AppErrorsCataloge.Unauthorized(
                    "Invalid credentials.",
                    _stringLocalizer[AuthMessage.InvalidCredentials]));

            if (user.IsEmailVerified)
            {
                return Result<bool>.Failure(AppErrorsCataloge.Validation(
                    "Email already verified.",
                    _stringLocalizer[AuthMessage.EmailAlreadyVerified]));
            }

            var VeificationCode =
                await _verificationTokenService.CreateOrRefreshAsync(
                    user.Id,
                    type,
                    15,
                    ct);

            await _emailService.SendEmailConfirmationAsync(
                 user.Email,
                 user.FullName,
                 VeificationCode.Code,
                 ct);

            _logger.LogInformation(
                "Verification code resent to {Email}",
                user.Email);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to resend verification code to {Email}",
                email);

            return Result<bool>.Failure(AppErrorsCataloge.Failure(
                "An unexpected error occurred. Please try again later.",
                _stringLocalizer[CommonMessage.UnexpectedError]));
        }
    }

    // ─────────────────────────────────────────────────────────
    // Login Log User Active 
    // ─────────────────────────────────────────────────────────
    private async Task<UserActivity> LogUserActivityAsync(
     Guid? userId,
     Guid? adminId,
     ActivityType activityType,
     string description,
     CancellationToken ct)
    {
        var ipAddress = _currentUser.IpAddress;

        var location = await _geoLocationService
            .GetLocationAsync(ipAddress, ct);

        var userActivity = UserActivity.Create(
            userId,
            adminId,
            activityType,
            description,
            ipAddress,
            _currentUser.UserAgent,
            location);


        return userActivity;
    }

    // ─────────────────────────────────────────────────────────
    // Validate Verification Code
    // ─────────────────────────────────────────────────────────
    private async Task<(VerificationCode Token, User User)> ValidateVerificationCodeAsync(
    string code,
    VerificationCodeType type,
    CancellationToken ct)
    {
        var verificationcode =
            await _readVerificationCode.FindFirstAsync(
                t => t.Code == code
                  && t.Type == type
                  && !t.IsUsed,
                ct);

        if (verificationcode is null)
            throw new DomainException(
                "Invalid verification code.",
                DomainMessagies.InvalidVerificationCode);

        if (verificationcode.IsExpired)
            throw new DomainException(
                "Verification Code has expired.",
                DomainMessagies.VerificationCodeExpired);

        if (verificationcode.IsUsed)
            throw new DomainException(
                "Code already used.",
                DomainMessagies.VerificationCodeAlreadyUsed);

        var user =
            await _readUser.GetByIdAsync(
                verificationcode.UserId,
                ct);

        if (user is null)
            throw new DomainException(
                "User not found.",
                "NotFound");

        return (verificationcode, user);
    }
}
