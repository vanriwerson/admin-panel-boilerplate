using Api.Models.Common;

namespace Api.Data.Seeds;

public static class SeedExtensions
{
    public static void SetAuditFields(this AuditableEntity entity)
    {
        var now = DateTime.UtcNow;

        entity.CreatedAt = now;
        entity.UpdatedAt = now;
    }

    public static void SetAuditFields<T>(this IEnumerable<T> entities)
        where T : AuditableEntity
    {
        var now = DateTime.UtcNow;

        foreach (var entity in entities)
        {
            entity.CreatedAt = now;
            entity.UpdatedAt = now;
        }
    }
}