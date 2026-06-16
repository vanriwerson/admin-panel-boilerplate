using Api.Interfaces.Data.Seeds;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Seeds;

public class SystemResourcesSeed : ISeed
{
    public async Task SeedAsync(ApiDbContext context)
    {
        if (await context.SystemResources.AnyAsync())
            return;

        var resources = new List<SystemResource>
        {
            new() { Name = "root", ExhibitionName = "Administrador" },
            new() { Name = "users", ExhibitionName = "Gerenciamento de Usuários" },
            new() { Name = "resources", ExhibitionName = "Recursos do Sistema" },
            new() { Name = "reports", ExhibitionName = "Auditoria do Sistema" }
        };

        resources.SetAuditFields();

        await context.SystemResources.AddRangeAsync(resources);
        await context.SaveChangesAsync();

        Console.WriteLine("Seed de recursos do sistema executado.");
    }
}