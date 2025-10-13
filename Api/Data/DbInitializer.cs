using Api.Helpers;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApiDbContext context)
        {
            if (await context.Users.AnyAsync())
                return; // já existem usuários, não faz nada

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

            // Hash das senhas e definir timestamps
            foreach (var user in users)
            {
                user.Password = PasswordHashing.Generate(user.Password);
                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;
            }

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            Console.WriteLine("Seed de 10 usuários fictícios executada.");
        }
    }
}
