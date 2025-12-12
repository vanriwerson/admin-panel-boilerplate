using Api.Dtos;
using Api.Helpers;
using Api.Middlewares;
using Api.Services.AuthServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Api.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AuthController : ControllerBase
  {
    private readonly LoginService _loginService;
    private readonly ExternalTokenService _externalTokenService;
    private readonly PasswordServices _passwordServices;

    public AuthController(
        LoginService loginService,
        ExternalTokenService externalTokenService,
        PasswordServices passwordServices)
    {
      _loginService = loginService;
      _externalTokenService = externalTokenService;
      _passwordServices = passwordServices;
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
      try
      {
        var loginResponse = await _loginService.LoginAsync(request.Identifier, request.Password);
        if (loginResponse == null)
          return Unauthorized(new { message = "Credenciais inválidas." });

        return Ok(loginResponse);
      }
      catch (Exception)
      {
        return StatusCode(500, new { message = "Erro ao processar login." });
      }
    }

    // POST api/auth/external
    [HttpPost("external")]
    public async Task<IActionResult> ExchangeExternalToken([FromBody] ExternalTokenRequestDto request)
    {
      try
      {
        var loginResponse = await _externalTokenService.ExchangeExternalTokenAsync(request.ExternalToken);
        if (loginResponse == null)
          return Unauthorized(new { message = "Não foi possível fazer login por redirecionamento" });

        return Ok(loginResponse);
      }
      catch (Exception)
      {
        return StatusCode(500, new { message = "Erro ao processar token externo." });
      }
    }

    // POST api/auth/password/request-new
    [HttpPost("password/request-new")]
    public async Task<IActionResult> RequestNewPassword([FromBody] RequestNewPasswordDto request)
    {
      try
      {
        await _passwordServices.RequestNewPasswordAsync(request.Email);
        return Ok(new { message = "Um link de redefinição de senha foi enviado para o e-mail informado." });
      }
      catch (AppException ex)
      {
        return StatusCode(ex.StatusCode, new { message = ex.Message });
      }
      catch (Exception)
      {
        return StatusCode(500, new { message = "Erro ao processar solicitação de redefinição de senha." });
      }
    }

    // POST api/auth/password/reset
    [HttpPost("password/reset")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
    {
      try
      {
        await _passwordServices.ResetPasswordAsync(request.Token, request.NewPassword);
        return Ok(new { message = "Senha redefinida com sucesso." });
      }
      catch (AppException ex)
      {
        return StatusCode(ex.StatusCode, new { message = ex.Message });
      }
      catch (Exception)
      {
        return StatusCode(500, new { message = "Erro ao redefinir senha." });
      }
    }
  }
}
