using Api.Interfaces.Security.Policies;
using Api.Security.Jwt;
using Api.Security.Passwords;
using Api.Security.Policies;
using Api.Settings;
using Resend;

namespace Api.Extensions.DependencyInjection;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        var resendSettings = SettingsFactory.ProvideResendSettings();

        services.AddHttpContextAccessor();

        services.AddScoped<CurrentUserContext>();

        services.AddScoped<IUserVisibilityPolicy, UserVisibilityPolicy>();

        services.AddScoped<SystemResourceVisibilityPolicy>();

        services.AddScoped<AccessPermissionPolicy>();

        services.AddHttpClient<ResendClient>();

        services.Configure<ResendClientOptions>(options =>
        {
            options.ApiToken = resendSettings.ApiKey;
        });

        services.AddTransient<ResendClient>();

        services.AddScoped<PasswordResetEmailService>();

        return services;
    }
}