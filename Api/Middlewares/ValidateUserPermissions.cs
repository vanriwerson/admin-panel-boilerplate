using System.Net;
using System.Text.Json;
using Api.Dtos;
using Api.Helpers;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Api.Middlewares
{
  public class ValidateUserPermissions
  {
    private readonly RequestDelegate _next;

    public ValidateUserPermissions(RequestDelegate next)
    {
      _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      var path = context.Request.Path.Value?.ToLower() ?? "";
      var method = context.Request.Method.ToUpper();

      if (method == "GET" || path.Contains("/auth/"))
      {
        await _next(context);
        return;
      }

      var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
      if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
      {
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        await context.Response.WriteAsync("Header Authorization ausente ou inválido.");
        return;
      }

      var token = authHeader.Substring("Bearer ".Length).Trim();
      var principal = JsonWebToken.Decode(token);

      var userPermissionIds = GetPermissionIdsFromToken(principal);

      // Usuário root (1) tem acesso total
      if (userPermissionIds.Contains(1))
      {
        await _next(context);
        return;
      }

      var requiredPermissions = EndpointPermissions.GetRequiredPermissions(path);
      if (requiredPermissions.Any() && !requiredPermissions.Any(rp => userPermissionIds.Contains(rp)))
      {
        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        await context.Response.WriteAsync("Acesso negado: você não possui permissão para este recurso.");
        return;
      }

      if (path.StartsWith("/users") && (method == "POST" || method == "PUT"))
      {
        var bodyPermissions = await GetPermissionsFromBodyAsync(context);
        if (bodyPermissions.Contains(1) || bodyPermissions.Contains(3))
        {
          context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
          await context.Response.WriteAsync("Acesso negado: não é permitido atribuir permissões root(1) ou systemResources(3).");
          return;
        }
      }

      await _next(context);
    }

    private static int[] GetPermissionIdsFromToken(System.Security.Claims.ClaimsPrincipal principal)
    {
      try
      {
        var permsClaim = principal.Claims.FirstOrDefault(c => c.Type == "permissions")?.Value;
        if (permsClaim != null)
        {
          var permissions = JsonSerializer.Deserialize<SystemResourceOptionDto[]>(permsClaim);
          return permissions?.Select(p => p.Id).ToArray() ?? Array.Empty<int>();
        }
      }
      catch { }
      return Array.Empty<int>();
    }

    private static async Task<int[]> GetPermissionsFromBodyAsync(HttpContext context)
    {
      context.Request.EnableBuffering();
      using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
      var body = await reader.ReadToEndAsync();
      context.Request.Body.Position = 0;

      try
      {
        using var jsonDoc = JsonDocument.Parse(body);
        if (jsonDoc.RootElement.TryGetProperty("permissions", out var permsElement) &&
            permsElement.ValueKind == JsonValueKind.Array)
        {
          return permsElement.EnumerateArray().Select(p => p.GetInt32()).ToArray();
        }
      }
      catch { }

      return Array.Empty<int>();
    }
  }

  public static class ValidateUserPermissionsExtensions
  {
    public static IApplicationBuilder UseValidateUserPermissions(this IApplicationBuilder app)
    {
      return app.UseMiddleware<ValidateUserPermissions>();
    }
  }
}
