using MediatR;
using Microsoft.Extensions.Logging;
using Review_Guard.Application.Abstractions.Email;
using Review_Guard.Domain.Events;

namespace Review_Guard.Application.Feature.Auth.EventHandlers;

public class UserRegisteredEventHandler
    : INotificationHandler<UserRegisteredEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<UserRegisteredEventHandler> _logger;

    public UserRegisteredEventHandler(
        IEmailService emailService,
        ILogger<UserRegisteredEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(UserRegisteredEvent notification, CancellationToken ct)
    {
        try
        {
            await _emailService.SendEmailConfirmationAsync(
                notification.Email,
                notification.FullName,
                notification.Code,
                ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send confirmation email");
        }
    }
}
