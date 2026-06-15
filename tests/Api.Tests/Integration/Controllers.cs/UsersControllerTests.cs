using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Data;
using Api.Dtos;
using Api.Models;
using Api.Tests.Integration.Infrastructure;
using FluentAssertions;

namespace Api.Tests.Integration.Controllers;

[Collection("PostgreSql")]
public class UsersControllerTests
{
    private readonly PostgreSqlTestFixture _fixture;

    public UsersControllerTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Create_Should_Return_201()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.SystemResources.Add(
            new SystemResource
            {
                Id = 2,
                Name = "USERS",
                ExhibitionName = "Usuários"
            });

        await context.SaveChangesAsync();

        await using var factory =
            new ApiWebApplicationFactory(
                _fixture.ConnectionString);

        var client = factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                TestJwtFactory.CreateRootToken());

        var dto = new UserCreateDto
        {
            Username = "newuser",
            Email = "newuser@email.com",
            Password = "123456",
            FullName = "Novo Usuário",
            PermissionIds = [2]
        };

        var response =
            await client.PostAsJsonAsync(
                "/api/users",
                dto);

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Update_Should_Return_200()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var resource = new SystemResource
        {
            Id = 2,
            Name = "USERS",
            ExhibitionName = "Usuários"
        };

        context.SystemResources.Add(resource);

        var user = new User
        {
            Username = "olduser",
            Email = "old@email.com",
            Password = "hash",
            FullName = "Old User",
            Active = true
        };

        context.Users.Add(user);

        await context.SaveChangesAsync();

        await using var factory =
            new ApiWebApplicationFactory(
                _fixture.ConnectionString);

        var client = factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                TestJwtFactory.CreateRootToken());

        var dto = new UserUpdateDto
        {
            Id = user.Id,
            Username = "updated",
            Email = "updated@email.com",
            FullName = "Updated User",
            PermissionIds = [2]
        };

        var response =
            await client.PutAsJsonAsync(
                $"/api/users/{user.Id}",
                dto);

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_Should_Return_204()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var user = new User
        {
            Username = "delete-me",
            Email = "delete@email.com",
            Password = "hash",
            FullName = "Delete Me",
            Active = true
        };

        context.Users.Add(user);

        await context.SaveChangesAsync();

        await using var factory =
            new ApiWebApplicationFactory(
                _fixture.ConnectionString);

        var client = factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                TestJwtFactory.CreateRootToken());

        var response =
            await client.DeleteAsync(
                $"/api/users/{user.Id}");

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.NoContent);
    }
}