using Api.Helpers;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Api.Middlewares
{
  public class RequireAuthorization
  {
    private readonly RequestDelegate _next;
    private readonly string _jwtSecret;

    public RequireAuthorization(RequestDelegate next)
    {
      _next = next;
      _jwtSecret = EnvLoader.GetEnv("JWT_SECRET_KEY");
    }

    public async Task InvokeAsync(HttpContext context)
    {
      var path = context.Request.Path.Value?.ToLower() ?? "";
      var method = context.Request.Method.ToUpper();

      if (path.Contains("/auth/"))
      {
        await _next(context);
        return;
      }

      if (method is "POST" or "PUT" or "DELETE")
      {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
        {
          context.Response.StatusCode = StatusCodes.Status401Unauthorized;
          await context.Response.WriteAsync("Header Authorization ausente ou inválido");
          return;
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();

        if (!JsonWebToken.Verify(token, _jwtSecret))
        {
          context.Response.StatusCode = StatusCodes.Status401Unauthorized;
          await context.Response.WriteAsync("Token inválido ou expirado");
          return;
        }
      }

      await _next(context);
    }
  }

  // ✅ Extensão para facilitar o uso no Program.cs
  public static class RequireAuthorizationExtensions
  {
    public static IApplicationBuilder UseRequireAuthorization(this IApplicationBuilder app)
    {
      return app.UseMiddleware<RequireAuthorization>();
    }
  }
}
