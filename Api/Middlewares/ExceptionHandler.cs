using System.Net;
using System.Text.Json;

namespace Api.Middlewares
{
  public class ExceptionHandler
  {
    private readonly RequestDelegate _next;

    public ExceptionHandler(RequestDelegate next)
    {
      _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      try
      {
        await _next(context);
      }
      catch (AppException ex)
      {
        context.Response.StatusCode = ex.StatusCode;
        context.Response.ContentType = "application/json";

        var result = JsonSerializer.Serialize(new { message = ex.Message });
        await context.Response.WriteAsync(result);
      }
      catch (Exception)
      {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var result = JsonSerializer.Serialize(new { message = "Ocorreu um erro inesperado." });
        await context.Response.WriteAsync(result);
      }
    }
  }

  // Extensão para Program.cs
  public static class ExceptionHandlerExtensions
  {
    public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder app)
    {
      return app.UseMiddleware<ExceptionHandler>();
    }
  }
}
