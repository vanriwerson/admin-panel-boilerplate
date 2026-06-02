using Api.Dtos;

namespace Api.Tests.TestData.Dtos;

public static class UserCreateDtoFactory
{
    public static UserCreateDto Create(
        Action<UserCreateDto>? overrideValues = null)
    {
        var dto = new UserCreateDto
        {
            Username = "bruno",
            Email = "bruno@email.com",
            Password = "123456",
            FullName = "Bruno Silva",
            PermissionIds = [1]
        };

        overrideValues?.Invoke(dto);

        return dto;
    }

    public static UserCreateDto WithoutPermissions()
        => Create(dto => dto.PermissionIds = []);

    public static UserCreateDto WithEmptyUsername()
        => Create(dto => dto.Username = "");

    public static UserCreateDto WithEmptyEmail()
        => Create(dto => dto.Email = "");

    public static UserCreateDto WithEmptyPassword()
        => Create(dto => dto.Password = "");

    public static UserCreateDto WithEmptyFullName()
        => Create(dto => dto.FullName = "");
}