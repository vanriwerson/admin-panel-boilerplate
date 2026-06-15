using Api.Models;
using System.Security.Claims;

namespace Api.Interfaces.Security.Auth;

public interface IAuthUserResolver
{
    Task<User?> FindByIdentifierAsync(
        string identifier);

    Task<User?> FindByExternalClaimsAsync(
        ClaimsPrincipal principal);
}