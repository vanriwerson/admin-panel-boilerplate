using System.Net;
using System.Net.Http.Json;
using Api.Models;
using Api.Security.Jwt;
using Api.Tests.Integration.Infrastructure;
using FluentAssertions;

namespace Api.Tests.Integration.Controllers;

[Collection("PostgreSql")]
public class PasswordControllerTests
{
    private readonly PostgreSqlTestFixture _fixture;

    public PasswordControllerTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task RequestNewPassword_Should_Return_200()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.Users.Add(
            new User
            {
                Username = "user",
                Email = "user@email.com",
                FullName = "User",
                Password = "hash"
            });

        await context.SaveChangesAsync();

        await using var factory =
            new ApiWebApplicationFactory(
                _fixture.ConnectionString);

        var client =
            factory.CreateClient();

        var response =
            await client.PostAsJsonAsync(
                "/api/password/request-new",
                new
                {
                    Email = "user@email.com"
                });

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ResetPassword_Should_Return_200()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var user = new User
        {
            Username = "user",
            Email = "user@email.com",
            FullName = "User",
            Password = "hash"
        };

        context.Users.Add(user);

        await context.SaveChangesAsync();

        var token =
            JwtServices.Create(
            [
                new("id", user.Id.ToString()),
                new("email", user.Email)
            ]);

        await using var factory =
            new ApiWebApplicationFactory(
                _fixture.ConnectionString);

        var client =
            factory.CreateClient();

        var response =
            await client.PostAsJsonAsync(
                "/api/password/reset",
                new
                {
                    Token = token,
                    NewPassword = "NovaSenha123"
                });

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
    }
}