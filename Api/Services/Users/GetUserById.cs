using Api.Dtos;
using Api.Interfaces.Repositories;
using Api.Middlewares;

namespace Api.Services.Users;

public class GetUserById
{
    private readonly IUserRepository _repository;

    public GetUserById(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserReadDto> ExecuteAsync(int id)
    {
        Guard.AgainstNonPositiveInt(id);

        var user = await _repository.GetByIdAsync(id)
            ?? throw new AppException("Usuário não encontrado.");

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
