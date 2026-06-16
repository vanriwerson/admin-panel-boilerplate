using Api.Data;

namespace Api.Interfaces.Data.Seeds;

public interface ISeed
{
    Task SeedAsync(ApiDbContext context);
}