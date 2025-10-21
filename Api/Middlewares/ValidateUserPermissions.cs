using System.Net;
using System.Text.Json;
using Api.Helpers;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

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

      if (method == "GET")
      {
        await _next(context);
        return;
      }

      if (path.Contains("/auth/"))
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
      var permissionIds = JsonWebToken.GetPermissionIds(principal);

      // ✅ Usuário root (1) tem acesso total
      if (permissionIds.Contains(1))
      {
        await _next(context);
        return;
      }

      // ✅ Regras específicas para endpoints de usuários
      if (path.Contains("/users"))
      {
        if (!permissionIds.Contains(2))
        {
          context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
          await context.Response.WriteAsync("Acesso negado: você não possui permissão para gerenciar usuários.");
          return;
        }

        // Protege contra criação/edição de usuários root (1) ou systemResources (3)
        if (method == "POST" || method == "PUT")
        {
          context.Request.EnableBuffering(); // Permite reler o body
          using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
          var body = await reader.ReadToEndAsync();
          context.Request.Body.Position = 0;

          try
          {
            using var jsonDoc = JsonDocument.Parse(body);
            if (jsonDoc.RootElement.TryGetProperty("permissions", out var permsElement) &&
                permsElement.ValueKind == JsonValueKind.Array)
            {
              var perms = permsElement.EnumerateArray().Select(p => p.GetInt32()).ToArray();

              if (perms.Contains(1) || perms.Contains(3))
              {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsync("Acesso negado: não é permitido atribuir permissões root(1) ou systemResources(3).");
                return;
              }
            }
          }
          catch (JsonException)
          {
            // Body inválido, deixa passar para o controller lidar com isso
          }
        }
      }

      if (path.Contains("/reports"))
      {
        if (!permissionIds.Contains(4))
        {
          context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
          await context.Response.WriteAsync("Acesso negado: você não possui permissão para gerar relatórios.");
          return;
        }
      }

      await _next(context);
    }
  }

  // 🔧 Extensão para facilitar o uso no Program.cs
  public static class ValidateUserPermissionsExtensions
  {
    public static IApplicationBuilder UseValidateUserPermissions(this IApplicationBuilder app)
    {
      return app.UseMiddleware<ValidateUserPermissions>();
    }
  }
}
