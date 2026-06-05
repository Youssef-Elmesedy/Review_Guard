using Review_Guard.API.Middleware;
using Review_Guard.Application.Abstractions.Services.CurrentUserService;

namespace Review_Guard.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();


        return services;
    }

}