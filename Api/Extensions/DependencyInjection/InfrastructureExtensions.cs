using Api.Helpers;
using Api.Security.Jwt;
using Api.Security.Passwords;
using Microsoft.Extensions.DependencyInjection;
using Resend;

namespace Api.Extensions.DependencyInjection;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<CurrentUserContext>();

        // --- Resend ---
        var resendApiKey = EnvLoader.GetEnv("RESEND_API_KEY");
        services.AddHttpClient<ResendClient>();
        services.Configure<ResendClientOptions>(options =>
        {
            options.ApiToken = resendApiKey;
        });
        services.AddTransient<ResendClient>();

        // Serviço de reset de senha
        services.AddScoped<PasswordResetEmailService>();

        return services;
    }
}
