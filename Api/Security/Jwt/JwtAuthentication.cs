using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Security.Jwt;

public class JwtAuthentication
{
    private readonly RequestDelegate _next;

    public JwtAuthentication(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(authHeader) &&
            authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();

            var handler = new JwtSecurityTokenHandler();

            if (handler.CanReadToken(token))
            {
                var jwt = handler.ReadJwtToken(token);

                var identity = new ClaimsIdentity(
                    jwt.Claims,
                    authenticationType: "Jwt"
                );

                context.User = new ClaimsPrincipal(identity);
            }
        }

        await _next(context);
    }
}

public static class JwtAuthenticationExtensions
{
    public static IApplicationBuilder UseJwtAuthentication(this IApplicationBuilder app)
    {
        return app.UseMiddleware<JwtAuthentication>();
    }
}