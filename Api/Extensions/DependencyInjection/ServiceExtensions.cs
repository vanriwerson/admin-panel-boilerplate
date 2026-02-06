using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Api.Extensions.DependencyInjection;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        ILogger logger
    )
    {
        var assembly = Assembly.GetExecutingAssembly();
        int count = 0;

        foreach (var type in assembly.GetTypes().Where(t =>
            t.IsClass &&
            t.Namespace != null &&
            (
                t.Namespace.StartsWith("Api.Services") ||
                t.Namespace.StartsWith("Api.Security.Auth") ||
                t.Namespace.StartsWith("Api.Auditing.Services")
            )
        ))
        {
            services.AddScoped(type);
            count++;
        }

        logger.LogInformation("{count} Services registrados com sucesso", count);

        return services;
    }
}
