using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Api.Security.Jwt;

namespace Api.Security.Jwt;

public class CurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetId()
    {
        var principal = GetPrincipal();
        return principal.GetUserId();
    }

    public IEnumerable<int> GetPermissionIds()
    {
        var principal = GetPrincipal();
        return principal.GetPermissionIds();
    }

    public string? GetIpAddress()
    {
        return _httpContextAccessor.HttpContext?
            .Connection
            .RemoteIpAddress?
            .ToString();
    }

    public string? GetUsername()
    {
        var principal = GetPrincipal();
        return principal.Identity?.Name;
    }

    private ClaimsPrincipal GetPrincipal()
    {
        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext não disponível");

        var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            throw new InvalidOperationException("Token JWT ausente no header Authorization");

        var token = authHeader["Bearer ".Length..].Trim();

        return JwtService.Validate(token);
    }
}
