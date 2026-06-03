using Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests.Integration.Infrastructure;

public static class DatabaseCleaner
{
    public static async Task ResetAsync(
        ApiDbContext context)
    {
        await context.Database.ExecuteSqlRawAsync("""
        TRUNCATE TABLE
            "access_permissions",
            "refresh_tokens",
            "system_logs",
            "system_resources",
            "users"
        RESTART IDENTITY CASCADE;
        """);
    }
}