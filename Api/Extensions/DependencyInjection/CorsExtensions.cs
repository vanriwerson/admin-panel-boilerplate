using Api.Settings;

namespace Api.Extensions.DependencyInjection;

public static class CorsExtensions
{
    public const string FrontendPolicy = "FrontendPolicy";

    public static IServiceCollection AddFrontendCors(
        this IServiceCollection services,
        FrontendSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        var origins = settings.Url
            .Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(url => url.Trim())
            .ToArray();

        services.AddCors(options =>
        {
            options.AddPolicy(
                FrontendPolicy,
                policy =>
                {
                    policy.WithOrigins(origins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
        });

        return services;
    }
}