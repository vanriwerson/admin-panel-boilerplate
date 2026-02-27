using Api.Interfaces;
using Api.Interfaces.Repositories;
using Api.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Api.Extensions.DependencyInjection;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(
        this IServiceCollection services,
        ILogger logger
    )
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISystemResourceRepository, SystemResourceRepository>();
        services.AddScoped<IAccessPermissionRepository, AccessPermissionRepository>();
        services.AddScoped<ISystemLogRepository, SystemLogRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        logger.LogInformation("Repositories registrados com sucesso");

        return services;
    }
}
