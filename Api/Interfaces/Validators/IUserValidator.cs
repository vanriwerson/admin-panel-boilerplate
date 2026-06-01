using Api.Dtos;
using Api.Models;

namespace Api.Interfaces.Validators;

public interface IUserValidator
{
    Task ValidateCreateAsync(UserCreateDto dto);
    Task ValidateUpdateAsync(UserUpdateDto dto, User user);
}