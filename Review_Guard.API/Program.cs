namespace Review_Guard.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Controllers
        builder.Services.AddControllers()
            .AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.PropertyNamingPolicy =
                    System.Text.Json.JsonNamingPolicy.CamelCase;

                opts.JsonSerializerOptions.Converters.Add(
                    new System.Text.Json.Serialization.JsonStringEnumConverter());

                opts.JsonSerializerOptions.DefaultIgnoreCondition =
                    System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            });

        // Allow trailing commas and comments in JSON for better error handling
        builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
        {
            options.JsonSerializerOptions.AllowTrailingCommas = true;
            options.JsonSerializerOptions.ReadCommentHandling =
                System.Text.Json.JsonCommentHandling.Skip;
        });

        // Localization
        builder.Services.AddLocalizationServices();

        // Caching
        builder.Services.AddMemoryCache();
        builder.Services.AddResponseCaching();

        // Core
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHttpContextAccessor();

        // Layers
        builder.Services.AddApiServices();
        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Logging.AddCustomLogging();

        // Cross-cutting
        builder.Services.AddJwtAuthentication(builder.Configuration);
        builder.Services.AddAuthorization(); // 
        builder.Services.AddSwaggerWithAuth();

        /// Configure forwarded headers to correctly capture client IP and protocol when behind a reverse proxy
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto;

            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        var app = builder.Build();

        // Apply pending migrations on startup
        try
        {
            app.ApplyMigrations();

            await AdminSeeder.SeedAsync(app.Services);

            await UserSeeder.SeedAsync(app.Services);

            await BusinessCategorySeeder.SeedAsync(app.Services);

            await BusinessSeeder.SeedAsync(app.Services);

            await BranchSeeder.SeedAsync(app.Services);

            await ProofSeeder.SeedAsync(app.Services);

            await ReviewSeeder.SeedAsync(app.Services);
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();

            logger.LogError(ex, "Migration failed");
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Log raw request body for debugging purposes
        //app.Use(async (ctx, next) =>
        //{
        //    ctx.Request.EnableBuffering();

        //    var body = await new StreamReader(ctx.Request.Body).ReadToEndAsync();
        //    Console.WriteLine($"RAW BODY: {body}");

        //    ctx.Request.Body.Position = 0;

        //    await next();
        //});

        // Use localization
        var localizationOptions =
            app.Services
               .GetRequiredService<
                    Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>()
               .Value;

        app.UseRequestLocalization(localizationOptions);

        app.UseHttpsRedirection();

        app.UseCors("AllowFrontend");

        app.UseMiddleware<GlobalExceptionMiddleware>();

        app.UseForwardedHeaders();

        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
