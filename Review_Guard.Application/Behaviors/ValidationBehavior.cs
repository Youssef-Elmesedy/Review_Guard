using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Review_Guard.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
     TRequest request,
     RequestHandlerDelegate<TResponse> next,
     CancellationToken ct)
    {
        if (!_validators.Any())
            return await next();

        var failures = _validators
            .Select(v => v.Validate(request))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return await next();

        var errors = string.Join("; ", failures.Select(f => f.ErrorMessage));

        _logger.LogWarning(
            "Validation failed for {RequestName}: {Errors}",
            typeof(TRequest).Name,
            errors);

        throw new ValidationException(failures);
    }
}
