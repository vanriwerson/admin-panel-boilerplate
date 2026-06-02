using Api.Dtos;
using Api.Interfaces.Repositories;
using Api.Interfaces.Security.Policies;
using Api.Mappers;
using Api.Middlewares;

namespace Api.Services.Users;

public class GetUserById
{
    private readonly IUserRepository _repository;
    private readonly IUserVisibilityPolicy _visibility;

    public GetUserById(
        IUserRepository repository,
        IUserVisibilityPolicy visibility
    )
    {
        _repository = repository;
        _visibility = visibility;
    }

    public async Task<UserReadDto> ExecuteAsync(int id)
    {
        Guard.AgainstNonPositiveInt(id);

        var user = await _repository.GetByIdAsync(id)
            ?? throw new AppException("Usuário não encontrado.");

        if (!_visibility.CanAccess(user))
            throw new AppException("Usuário não encontrado.");

        return UserMapper.MapToUserReadDto(user);
    }
}
