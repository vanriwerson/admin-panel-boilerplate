using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Api.Helpers
{
  public class CurrentAuthUser
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentAuthUser(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public int GetId()
    {
      var httpContext = _httpContextAccessor.HttpContext
                        ?? throw new InvalidOperationException("HttpContext não disponível");

      var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
      if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
        throw new InvalidOperationException("Token JWT ausente no header Authorization");

      var token = authHeader.Substring("Bearer ".Length).Trim();

      var principal = JsonWebToken.Decode(token);

      var idClaim = principal.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
      if (string.IsNullOrEmpty(idClaim))
        throw new InvalidOperationException("Claim 'id' não encontrada no token");

      if (!int.TryParse(idClaim, out var userId))
        throw new InvalidOperationException("Claim 'id' inválida");

      return userId;
    }
  }
}
