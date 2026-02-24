using Api.Auditing;
using Api.Auditing.Services;
using Api.Dtos;
using Api.Security.Jwt;
using Api.Security.Passwords;
using System.Security.Claims;

namespace Api.Security.Auth;

public class AuthServices
{
    private readonly AuthUserResolver _resolver;
    private readonly LoginResponseFactory _loginResponseFactory;
    private readonly CreateSystemLog _log;

    public AuthServices(
        AuthUserResolver resolver,
        LoginResponseFactory loginResponseFactory,
        CreateSystemLog log)
    {
        _resolver = resolver;
        _loginResponseFactory = loginResponseFactory;
        _log = log;
    }

    public async Task<LoginResponseDto?> LoginAsync(string identifier, string password)
    {
        var user = await _resolver.FindByIdentifierAsync(identifier);

        if (user == null || !PasswordHash.Verify(password, user.Password))
            return null;

        var response = _loginResponseFactory.CreateResponse(user);

        await _log.ExecuteAsync(
            SystemLogActionFactory.Login(user.Username),
            user.Id,
            user.Username
        );

        return response;
    }

    public async Task<LoginResponseDto?> LoginWithExternalTokenAsync(string externalToken)
    {
        ClaimsPrincipal principal;

        try
        {
            principal = JwtServices.Validate(externalToken);
        }
        catch
        {
            return null;
        }

        var user = await _resolver.FindByExternalClaimsAsync(principal);
        if (user == null)
            return null;

        var response = _loginResponseFactory.CreateResponse(user);

        await _log.ExecuteAsync(
            SystemLogActionFactory.ExternalLogin(user.Username),
            user.Id,
            user.Username
        );

        return response;
    }

    public async Task LogoutAsync(int userId, string username)
    {
        await _log.ExecuteAsync(
            SystemLogActionFactory.Logout(username),
            userId,
            username
        );
    }
}
