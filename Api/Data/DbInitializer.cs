using Api.Data.Seeds;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public static class DbInitializer
{
    public static async Task SeedAllAsync(ApiDbContext context)
    {
        await SeedRunner.RunAsync(context);
    }
}

public static class ApplicationBuilderExtensions
{
    public static async Task UseDbInitializerAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ApiDbContext>();

        await context.Database.MigrateAsync();

        await DbInitializer.SeedAllAsync(context);
    }
}