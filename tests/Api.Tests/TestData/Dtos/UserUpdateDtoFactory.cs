using Api.Dtos;

namespace Api.Tests.TestData.Dtos;

public static class UserUpdateDtoFactory
{
    public static UserUpdateDto Create(
        Action<UserUpdateDto>? overrideValues = null)
    {
        var dto = new UserUpdateDto
        {
            Id = 1,
            Username = "novo.username",
            Email = "novo@email.com",
            FullName = "Novo Nome",
            Password = "123456"
        };

        overrideValues?.Invoke(dto);

        return dto;
    }

    public static UserUpdateDto WithoutUsername()
        => Create(dto => dto.Username = null);

    public static UserUpdateDto WithoutEmail()
        => Create(dto => dto.Email = null);

    public static UserUpdateDto WithoutFullName()
        => Create(dto => dto.FullName = null);

    public static UserUpdateDto WithoutPassword()
        => Create(dto => dto.Password = null);
}