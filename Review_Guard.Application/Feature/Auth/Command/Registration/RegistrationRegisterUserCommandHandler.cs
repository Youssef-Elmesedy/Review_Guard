using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.Auth.DTOs.Responses;
using Review_Guard.Application.Feature.Auth.Services;

namespace Review_Guard.Application.Feature.Auth.Command.Registration
{
    public sealed class RegistrationRegisterUserCommandHandler : IRequestHandler<RegistrationRegisterUserCommand, Result<AuthResponseDto>>
    {
        public readonly IAuthService _authService;

        public RegistrationRegisterUserCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result<AuthResponseDto>> Handle(RegistrationRegisterUserCommand request, CancellationToken ct = default)
        {
            var result = await _authService.RegisterUserAsync(request.RegisterUserDto, ct);

            return result;
        }
    }
}
