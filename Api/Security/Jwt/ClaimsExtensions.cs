using System.Security.Claims;

namespace Api.Security.Jwt;

public static class ClaimsExtensions
{
    public static int GetUserId(this ClaimsPrincipal principal)
        => int.Parse(principal.FindFirstValue("id")!);

    public static IEnumerable<int> GetPermissionIds(
        this ClaimsPrincipal principal
    )
        => principal.Claims
            .Where(c => c.Type == "permission")
            .Select(c => int.Parse(c.Value));
}
