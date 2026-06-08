using Api.Dtos;
using Api.Models;

namespace Api.Interfaces.Security.Auth;

public interface ILoginResponseFactory
{
    Task<LoginResponseDto> CreateResponseAsync(
        User user);
}