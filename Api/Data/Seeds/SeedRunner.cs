using Api.Interfaces.Data.Seeds;

namespace Api.Data.Seeds;

public static class SeedRunner
{
    public static async Task RunAsync(ApiDbContext context)
    {
        var seeds = new List<ISeed>
        {
            new SystemResourcesSeed(),
            new RootUserSeed(),
            new UsersSeed()
        };

        foreach (var seed in seeds)
        {
            await seed.SeedAsync(context);
        }
    }
}