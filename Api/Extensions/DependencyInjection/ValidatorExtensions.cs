using Api.Validations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Api.Extensions.DependencyInjection;

public static class ValidatorExtensions
{
    public static IServiceCollection AddValidators(
        this IServiceCollection services,
        ILogger logger
    )
    {
        services.AddScoped<UserValidator>();

        logger.LogInformation("Validators registrados com sucesso");

        return services;
    }
}
