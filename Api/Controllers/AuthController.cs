using Api.Dtos;
using Api.Helpers;
using Api.Services.AuthServices;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace Api.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AuthController : ControllerBase
  {
    private readonly LoginService _loginService;
    private readonly ExternalTokenService _externalTokenService;

    public AuthController(LoginService loginService, ExternalTokenService externalTokenService)
    {
      _loginService = loginService;
      _externalTokenService = externalTokenService;
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
      try
      {
        var token = await _loginService.LoginAsync(request.Identifier, request.Password);
        if (token == null)
          return Unauthorized(new { message = "Credenciais inválidas." });

        return Ok(new
        {
          token
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "Erro ao processar login.", details = ex.Message });
      }
    }

    // POST api/auth/external
    [HttpPost("external")]
    public async Task<IActionResult> ExchangeExternalToken([FromBody] ExternalTokenRequest request)
    {
      try
      {
        var newToken = await _externalTokenService.ExchangeExternalTokenAsync(request.ExternalToken);
        if (newToken == null)
          return Unauthorized(new { message = "Não foi possível fazer login por redirecionamento" });

        return Ok(new
        {
          token = newToken
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "Erro ao processar token externo.", details = ex.Message });
      }
    }
  }

  // DTOs para requests
  public class LoginRequest
  {
    public string Identifier { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
  }

  public class ExternalTokenRequest
  {
    public string ExternalToken { get; set; } = string.Empty;
  }
}
