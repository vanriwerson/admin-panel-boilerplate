using Api.Helpers;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public static class DbInitializer
    {
        public static async Task SeedUsersAsync(ApiDbContext context)
        {
            var runSeed = Environment.GetEnvironmentVariable("RUN_USERS_SEED");
            if (!string.Equals(runSeed, "true", StringComparison.OrdinalIgnoreCase))
                return;

            if (await context.Users.AnyAsync())
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
                user.Password = PasswordHashing.Generate(user.Password);
                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;
            }

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            Console.WriteLine("Seed de usuários de teste executada.");
        }

        public static async Task SeedSystemResourcesAsync(ApiDbContext context)
        {
            if (await context.SystemResources.AnyAsync())
                return;

            var resources = new List<SystemResource>
            {
                new SystemResource { Name = "root", ExhibitionName = "Administrador" },
                new SystemResource { Name = "users", ExhibitionName = "Gerenciamento de Usuários" },
                new SystemResource { Name = "resources", ExhibitionName = "Recursos do Sistema" },
                new SystemResource { Name = "reports", ExhibitionName = "Auditoria do Sistema" },
            };

            foreach (var resource in resources)
            {
                resource.CreatedAt = DateTime.UtcNow;
                resource.UpdatedAt = DateTime.UtcNow;
            }

            await context.SystemResources.AddRangeAsync(resources);
            await context.SaveChangesAsync();

            Console.WriteLine("Seed de system resources executada.");
        }

        public static async Task CreateRootAsync(ApiDbContext context)
        {
            if (await context.Users.AnyAsync(u => u.Username == "root"))
                return;

            var rootResource = await context.SystemResources.FirstOrDefaultAsync(r => r.Name == "root");
            if (rootResource == null)
            {
                rootResource = new SystemResource
                {
                    Name = "root",
                    ExhibitionName = "Administrador",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await context.SystemResources.AddAsync(rootResource);
                await context.SaveChangesAsync();
            }

            var rootUser = new User
            {
                Username = "root",
                Email = "root@admin.com",
                Password = PasswordHashing.Generate("root1234"), // trocar para senha segura em produção
                FullName = "Administrador",
                Active = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(rootUser);
            await context.SaveChangesAsync();

            var accessPermission = new AccessPermission
            {
                UserId = rootUser.Id,
                SystemResourceId = rootResource.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.AccessPermissions.AddAsync(accessPermission);
            await context.SaveChangesAsync();

            Console.WriteLine("Usuário root criado com sucesso.");
        }

        public static async Task SeedAllAsync(ApiDbContext context)
        {
            await SeedUsersAsync(context);
            await SeedSystemResourcesAsync(context);
            await CreateRootAsync(context);
        }
    }
}
