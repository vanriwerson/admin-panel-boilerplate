using System.Net;
using System.Net.Http.Json;
using Api.Models;
using Api.Security.Passwords;
using Api.Tests.Integration.Infrastructure;
using FluentAssertions;

namespace Api.Tests.Integration.Controllers;

[Collection("PostgreSql")]
public class AuthControllerTests
{
    private readonly PostgreSqlTestFixture _fixture;

    public AuthControllerTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Login_Should_Return_200()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var user = new User
        {
            Username = "root",
            Email = "root@email.com",
            FullName = "Root",
            Password =
                PasswordHash.Generate("123456")
        };

        context.Users.Add(user);

        await context.SaveChangesAsync();

        await using var factory =
            new ApiWebApplicationFactory(
                _fixture.ConnectionString);

        var client =
            factory.CreateClient();

        var response =
            await client.PostAsJsonAsync(
                "/api/auth/login",
                new
                {
                    Identifier = "root",
                    Password = "123456"
                });

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Login_Should_Return_401_When_Invalid_Credentials()
    {
        await using var factory =
            new ApiWebApplicationFactory(
                _fixture.ConnectionString);

        var client =
            factory.CreateClient();

        var response =
            await client.PostAsJsonAsync(
                "/api/auth/login",
                new
                {
                    Identifier = "fake",
                    Password = "wrong"
                });

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }
}