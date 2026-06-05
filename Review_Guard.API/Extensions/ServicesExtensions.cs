using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Console;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Review_Guard.API.Localization;
using Review_Guard.API.Logging;
using Review_Guard.API.Swagger;
using System.Globalization;
using System.Text;

namespace Review_Guard.API.Extensions;

public static class ServicesExtensions
{
    public static readonly string[] SupportedCultures = ["en", "ar"];
    public static readonly string DefaultCulture = "en";

    // ────────────────────────────st   ─────────────────────────────
    // Localization
    // ─────────────────────────────────────────────────────────
    public static IServiceCollection AddLocalizationServices(
        this IServiceCollection services)
    {
        services.AddLocalization();
        services.AddSingleton<JsonLocalizationStore>();
        services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();

        services.Configure<RequestLocalizationOptions>(opts =>
        {
            var cultures = SupportedCultures
                .Select(c => new CultureInfo(c))
                .ToList();

            opts.DefaultRequestCulture = new RequestCulture(DefaultCulture);
            opts.SupportedCultures = cultures;
            opts.SupportedUICultures = cultures;

            opts.RequestCultureProviders =
            [
                new QueryStringRequestCultureProvider { QueryStringKey = "culture" },
                new CookieRequestCultureProvider(),
                new AcceptLanguageHeaderRequestCultureProvider()
            ];
        });

        return services;
    }

    // ─────────────────────────────────────────────────────────
    // JWT Authentication
    // ─────────────────────────────────────────────────────────
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services, IConfiguration config)
    {
        var jwtSection = config.GetSection("Jwt");
        var secret = jwtSection["Secret"]
            ?? throw new InvalidOperationException("JWT Secret is not configured.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidateIssuer = true,
                ValidIssuer = jwtSection["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSection["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = ctx =>
                {
                    if (ctx.Exception is SecurityTokenExpiredException)
                        ctx.Response.Headers["Token-Expired"] = "true";
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }

    // ─────────────────────────────────────────────────────────
    // Swagger
    // ─────────────────────────────────────────────────────────
    public static IServiceCollection AddSwaggerWithAuth(
        this IServiceCollection services)
    {
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Review Guard API",
                Description = "An ASP.NET Core Web API for managing reviews and users.",
                Contact = new OpenApiContact
                {
                    Name = "Youssef Mostafa Elmesedy",
                    Email = "yousefelmesedy6@gmail.com",
                }
            });

            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter: Bearer {your-token}"
            });

            opt.OperationFilter<AuthorizeCheckOperationFilter>();
            opt.OperationFilter<CultureParameterOperationFilter>();

            // XML Comments
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                opt.IncludeXmlComments(xmlPath);
        });

        // ─────────────────────────────────────────────────────────
        // CORS Allowing Frontend (for development purposes, adjust for production)
        // ─────────────────────────────────────────────────────────
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
            });
        });

        return services;
    }



}
// Extension method for configuring custom logging
public static class LoggingExtensions
{
    public static ILoggingBuilder AddCustomLogging(
        this ILoggingBuilder builder)
    {
        builder.ClearProviders();

        builder.AddConsoleFormatter<
            ColoredConsoleFormatter,
            ConsoleFormatterOptions>();

        builder.AddConsole(options =>
        {
            options.FormatterName = "colored";
        });

        return builder;
    }
}
