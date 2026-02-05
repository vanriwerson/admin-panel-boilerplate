using Api.Security.Jwt;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Extensions.DependencyInjection;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<CurrentUserContext>();

        return services;
    }
}
