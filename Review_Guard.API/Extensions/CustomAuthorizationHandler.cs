using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.Extensions.Localization;
using Review_Guard.Application.Common;
using Review_Guard.Application.Common.CommonMessages;
using Review_Guard.Application.Common.ResultPattern;

public class CustomAuthorizationResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();
    private readonly IStringLocalizer<CustomAuthorizationResultHandler> _localizer;

    public CustomAuthorizationResultHandler(IStringLocalizer<CustomAuthorizationResultHandler> localizer)
    {
        _localizer = localizer;
    }

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult result)
    {
        // 403 Forbidden
        if (result.Forbidden)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var response = Result.Failure(
                AppErrorsCataloge.Unauthorized("403", _localizer[CommonMessage.Forbidden])
            );

            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        // 401 Unauthorized
        if (result.Challenged)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            var response = Result.Failure(
                AppErrorsCataloge.Unauthorized("401", _localizer[CommonMessage.Unauthorized])
            );

            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        await _defaultHandler.HandleAsync(next, context, policy, result);
    }
}

public static class CustomAuthorizationHandlerExtensions
{
    public static IServiceCollection AddCustomAuthorizationHandler(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthorizationResultHandler>();
        return services;
    }
}