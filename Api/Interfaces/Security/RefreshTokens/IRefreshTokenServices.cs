using Api.Models;

namespace Api.Interfaces.Security.RefreshTokens;

public interface IRefreshTokenServices
{
    Task<(string rawToken, RefreshToken entity)> GenerateAsync(
        User user);

    Task<RefreshToken?> ValidateAsync(
        string rawToken);

    Task RevokeAsync(
        RefreshToken token);

    Task RevokeAllForUserAsync(
        int userId);

    Task<(string rawToken, RefreshToken newEntity)> RotateAsync(
        RefreshToken oldToken);
}