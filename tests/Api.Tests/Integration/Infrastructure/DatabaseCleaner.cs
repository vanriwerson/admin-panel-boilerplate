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
            "users",
            "refresh_tokens",
            "access_permissions"
        RESTART IDENTITY CASCADE;
        """);
    }
}