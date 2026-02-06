using Api.Dtos;
using Api.Security.Auth;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthServices _authServices;

    public AuthController(AuthServices authServices)
    {
        _authServices = authServices;
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var response = await _authServices.LoginAsync(
                request.Identifier,
                request.Password
            );

            if (response == null)
                return Unauthorized(new { message = "Credenciais inválidas." });

            return Ok(response);
        }
        catch
        {
            return StatusCode(
                500,
                new { message = "Erro ao processar login." }
            );
        }
    }

    // POST api/auth/external
    [HttpPost("external")]
    public async Task<IActionResult> LoginWithExternalToken(
        [FromBody] ExternalTokenRequestDto request
    )
    {
        try
        {
            var response =
                await _authServices.LoginWithExternalTokenAsync(
                    request.ExternalToken
                );

            if (response == null)
                return Unauthorized(new
                {
                    message = "Não foi possível autenticar com o token externo."
                });

            return Ok(response);
        }
        catch
        {
            return StatusCode(
                500,
                new { message = "Erro ao processar autenticação externa." }
            );
        }
    }

    // TODO: POST api/auth/logout
}
