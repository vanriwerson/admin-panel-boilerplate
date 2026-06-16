using Api.Interfaces.Data.Seeds;
using Api.Models;
using Api.Security.Passwords;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Seeds;

public class RootUserSeed : ISeed
{
    public async Task SeedAsync(ApiDbContext context)
    {
        if (await context.Users.AnyAsync(u => u.Username == "root"))
            return;

        var rootResource = await context.SystemResources
            .FirstAsync(r => r.Name == "root");

        var rootUser = new User
        {
            Username = "root",
            Email = "root@admin.com",
            Password = PasswordHash.Generate("root1234"),
            FullName = "Administrador",
            Active = true
        };

        rootUser.SetAuditFields();

        await context.Users.AddAsync(rootUser);
        await context.SaveChangesAsync();

        await context.AccessPermissions.AddAsync(new AccessPermission
        {
            UserId = rootUser.Id,
            SystemResourceId = rootResource.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await context.SaveChangesAsync();

        Console.WriteLine("Usuário root criado com sucesso.");
    }
}