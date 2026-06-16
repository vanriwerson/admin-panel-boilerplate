using Api.Interfaces.Data.Seeds;
using Api.Models;
using Api.Security.Passwords;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Seeds;

public class UsersSeed : ISeed
{
    public async Task SeedAsync(ApiDbContext context)
    {
        var runSeed = Environment.GetEnvironmentVariable("RUN_USERS_SEED");

        if (!string.Equals(runSeed, "true",
            StringComparison.OrdinalIgnoreCase))
            return;

        if (await context.Users.AnyAsync(u => u.Username != "root"))
            return;

        var users = new List<User>
        {
            new User { Username = "alice", Email = "alice@test.com", Password = "123456", FullName = "Alice Wonderland" },
            new User { Username = "bob", Email = "bob@test.com", Password = "123456", FullName = "Bob Builder" },
            new User { Username = "carol", Email = "carol@test.com", Password = "123456", FullName = "Carol Singer" },
            new User { Username = "dave", Email = "dave@test.com", Password = "123456", FullName = "Dave Grohl" },
            new User { Username = "eve", Email = "eve@test.com", Password = "123456", FullName = "Eve Online" },
            new User { Username = "frank", Email = "frank@test.com", Password = "123456", FullName = "Frank Ocean" },
            new User { Username = "grace", Email = "grace@test.com", Password = "123456", FullName = "Grace Hopper" },
            new User { Username = "heidi", Email = "heidi@test.com", Password = "123456", FullName = "Heidi Klum" },
            new User { Username = "ivan", Email = "ivan@test.com", Password = "123456", FullName = "Ivan Drago" },
            new User { Username = "judy", Email = "judy@test.com", Password = "123456", FullName = "Judy Hopps" },
        };

        foreach (var user in users)
        {
            user.Password = PasswordHash.Generate(user.Password);
            user.SetAuditFields();
        }


        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        Console.WriteLine("Seed de usuários fictícios executada.");
    }
}