using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Api.Data;
using Api.Middlewares;
using Api.Models;
using Api.Helpers;

namespace Api.Services.AuthServices
{
  public class PasswordServices
  {
    private readonly ApiDbContext _context;

    public PasswordServices(ApiDbContext context)
    {
      _context = context;
    }

    public async Task RequestNewPasswordAsync(string email)
    {
      if (string.IsNullOrWhiteSpace(email))
        throw new AppException("Email inválido.", (int)HttpStatusCode.BadRequest);

      var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

      if (user == null)
        throw new AppException("Email não cadastrado.", (int)HttpStatusCode.NotFound);

      var claims = new[]
      {
                new System.Security.Claims.Claim("userId", user.Id.ToString()),
                new System.Security.Claims.Claim("email", user.Email)
            };

      var token = JsonWebToken.Create(claims, expireMinutes: 15);

      var webAppUrl = EnvLoader.GetEnv("WEB_APP_URL");
      var resetLink = $"{webAppUrl.TrimEnd('/')}/reset-password?token={token}";

      // Incluir serviço de envio de email
      Console.WriteLine($"[DEBUG] Link de redefinição enviado para {email}: {resetLink}");
    }


    public async Task ResetPasswordAsync(string token, string newPassword)
    {
      if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword))
        throw new AppException("Token ou senha inválidos.", (int)HttpStatusCode.BadRequest);

      System.Security.Claims.ClaimsPrincipal principal;
      try
      {
        principal = JsonWebToken.Decode(token, validateLifetime: true);
      }
      catch
      {
        throw new AppException("Token inválido ou expirado.", (int)HttpStatusCode.Unauthorized);
      }

      var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;

      if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        throw new AppException("Token inválido.", (int)HttpStatusCode.Unauthorized);

      var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

      if (user == null)
        throw new AppException("Usuário não encontrado.", (int)HttpStatusCode.NotFound);

      // Atualiza a senha
      user.Password = PasswordHashing.Generate(newPassword);
      user.UpdatedAt = DateTime.UtcNow;

      _context.Users.Update(user);
      await _context.SaveChangesAsync();
    }
  }
}
