using Api.Dtos;
using Api.Interfaces.Repositories;
using Api.Middlewares;
using Api.Security.Policies;

namespace Api.Services.Users;

public class GetUserById
{
    private readonly IUserRepository _repository;
    private readonly UserVisibilityPolicy _visibility;

    public GetUserById(
        IUserRepository repository,
        UserVisibilityPolicy visibility
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

        return new UserReadDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Permissions = user.AccessPermissions
                .Select(ap => new SystemResourceSelectDto
                {
                    Id = ap.SystemResource.Id,
                    ExhibitionName = ap.SystemResource.ExhibitionName
                })
                .ToList()
        };
    }
}
