using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Review_Guard.Application.Behaviors;
using Review_Guard.Application.Feature.Auth.Command.Registration;
using System.Reflection;

namespace Review_Guard.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = typeof(RegisterUserCommandValidator).Assembly;
        //Console.WriteLine($"assembly: {assembly}\n");
        //Console.WriteLine(typeof(RegisterUserValidator).Assembly.FullName);
        // ── MediatR + Pipeline Behaviors ──────────────────────────────────────
        // Order: Logging → Validation → Caching → Performance
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        });

        // ── FluentValidation ──────────────────────────────────────────────────
        try
        {
            services.AddValidatorsFromAssembly(assembly);
        }
        catch (ReflectionTypeLoadException ex)
        {
            foreach (var loaderEx in ex.LoaderExceptions)
                Console.WriteLine(loaderEx!.ToString());
            throw;
        }

        return services;
    }
}
