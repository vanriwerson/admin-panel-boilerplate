using Api.Data;
using Api.Settings;
using Microsoft.EntityFrameworkCore;

namespace Api.Extensions.DependencyInjection;

public static class DatabaseExtensions
{
    public static IServiceCollection ConnectToDatabase(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<ApiDbContext>(options =>
            options.UseNpgsql(
                connectionString
            )
        );

        return services;
    }
}