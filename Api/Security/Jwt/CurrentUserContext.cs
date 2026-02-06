using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Api.Security.Jwt;

public class CurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User =>
        _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated == true;

    public int? GetId()
    {
        var claim = User?.FindFirst("id");
        return claim != null && int.TryParse(claim.Value, out var id)
            ? id
            : null;
    }

    public string? GetUsername()
    {
        // prioridade: claim explícita
        return User?.FindFirst("username")?.Value
               // fallback padrão do ASP.NET
               ?? User?.FindFirst(ClaimTypes.Name)?.Value;
    }

    public string? GetIpAddress()
    {
        return _httpContextAccessor.HttpContext?
            .Connection?
            .RemoteIpAddress?
            .ToString();
    }
}
