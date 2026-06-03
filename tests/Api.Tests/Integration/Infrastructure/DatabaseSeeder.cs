using Api.Data;
using Api.Models;
using Api.Security.Permissions;

namespace Api.Tests.Integration.Infrastructure;

public static class DatabaseSeeder
{
    public static async Task SeedPermissionsAsync(
        ApiDbContext context)
    {
        if (context.SystemResources.Any())
            return;

        context.SystemResources.AddRange(
            new SystemResource
            {
                Id = BasePermissions.ROOT,
                Name = "ROOT",
                ExhibitionName = "ROOT",
                Active = true
            },
            new SystemResource
            {
                Id = BasePermissions.USERS,
                Name = "USERS",
                ExhibitionName = "USERS",
                Active = true
            },
            new SystemResource
            {
                Id = BasePermissions.RESOURCES,
                Name = "RESOURCES",
                ExhibitionName = "RESOURCES",
                Active = true
            },
            new SystemResource
            {
                Id = BasePermissions.REPORTS,
                Name = "REPORTS",
                ExhibitionName = "REPORTS",
                Active = true
            });

        await context.SaveChangesAsync();
    }
}