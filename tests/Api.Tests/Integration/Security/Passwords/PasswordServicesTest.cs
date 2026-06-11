using System.Security.Claims;
using Api.Interfaces.Auditing.Services;
using Api.Interfaces.Security.Passwords;
using Api.Middlewares;
using Api.Models;
using Api.Security.Jwt;
using Api.Security.Passwords;
using Api.Settings;
using Api.Tests.Integration.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;

namespace Api.Tests.Integration.Security.Passwords;

[Collection("PostgreSql")]
public class PasswordServicesTests
{
    private readonly PostgreSqlTestFixture _fixture;

    private static IOptions<FrontendSettings>
    CreateFrontendSettings()
    {
        return Options.Create(
            new FrontendSettings
            {
                Url = "http://localhost:5173"
            });
    }

    public PasswordServicesTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;

        const string secretKey =
            "unit-tests-secret-key-with-32-chars";

        Environment.SetEnvironmentVariable(
            "JWT_SECRET_KEY",
            secretKey);

        JwtServices.Initialize(
            new Api.Settings.JwtSettings
            {
                SecretKey = secretKey
            });
    }

    [Fact]
    public async Task RequestNewPasswordAsync_Should_Throw_When_Email_Is_Empty()
    {
        await using var context =
            _fixture.CreateContext();

        var logMock =
            new Mock<ICreateSystemLog>();

        var emailMock =
            new Mock<IPasswordResetEmailService>();

        var service = new PasswordServices(
            context,
            logMock.Object,
            emailMock.Object,
            CreateFrontendSettings());

        var act = () =>
            service.RequestNewPasswordAsync("");

        var exception = await act.Should()
            .ThrowAsync<AppException>();

        exception.Which.Message
            .Should()
            .Be("Email inválido.");
    }

    [Fact]
    public async Task RequestNewPasswordAsync_Should_Throw_When_Email_Does_Not_Exist()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var logMock =
            new Mock<ICreateSystemLog>();

        var emailMock =
            new Mock<IPasswordResetEmailService>();

        var service = new PasswordServices(
            context,
            logMock.Object,
            emailMock.Object,
            CreateFrontendSettings());

        var act = () =>
            service.RequestNewPasswordAsync(
                "naoexiste@email.com");

        var exception = await act.Should()
            .ThrowAsync<AppException>();

        exception.Which.Message
            .Should()
            .Be("Email não cadastrado.");
    }

    [Fact]
    public async Task RequestNewPasswordAsync_Should_Send_Email_And_Create_Log()
    {
        Environment.SetEnvironmentVariable(
            "WEB_APP_URL",
            "http://localhost:5173");

        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var user = new User
        {
            Username = "bruno",
            Email = "bruno@email.com",
            FullName = "Bruno",
            Password = "hash"
        };

        context.Users.Add(user);

        await context.SaveChangesAsync();

        var logMock =
            new Mock<ICreateSystemLog>();

        var emailMock =
            new Mock<IPasswordResetEmailService>();

        var service = new PasswordServices(
            context,
            logMock.Object,
            emailMock.Object,
            CreateFrontendSettings());

        await service.RequestNewPasswordAsync(
            user.Email);

        emailMock.Verify(
            x => x.SendEmailAsync(
                user.Email,
                It.IsAny<string>()),
            Times.Once);

        logMock.Verify(
            x => x.ExecuteAsync(
                It.Is<string>(action =>
                    action.Contains(
                        "solicitou reset de senha")),
                user.Id,
                null,
                null),
            Times.Once);
    }

    [Fact]
    public async Task ResetPasswordAsync_Should_Throw_When_Token_Is_Empty()
    {
        await using var context =
            _fixture.CreateContext();

        var logMock =
            new Mock<ICreateSystemLog>();

        var emailMock =
            new Mock<IPasswordResetEmailService>();

        var service = new PasswordServices(
            context,
            logMock.Object,
            emailMock.Object,
            CreateFrontendSettings());

        var act = () =>
            service.ResetPasswordAsync(
                "",
                "NovaSenha123");

        var exception = await act.Should()
            .ThrowAsync<AppException>();

        exception.Which.Message
            .Should()
            .Be("Token ou senha inválidos.");
    }

    [Fact]
    public async Task ResetPasswordAsync_Should_Throw_When_Password_Is_Empty()
    {
        await using var context =
            _fixture.CreateContext();

        var logMock =
            new Mock<ICreateSystemLog>();

        var emailMock =
            new Mock<IPasswordResetEmailService>();

        var service = new PasswordServices(
            context,
            logMock.Object,
            emailMock.Object,
            CreateFrontendSettings());

        var act = () =>
            service.ResetPasswordAsync(
                "token",
                "");

        var exception = await act.Should()
            .ThrowAsync<AppException>();

        exception.Which.Message
            .Should()
            .Be("Token ou senha inválidos.");
    }

    [Fact]
    public async Task ResetPasswordAsync_Should_Throw_When_Token_Is_Invalid()
    {
        await using var context =
            _fixture.CreateContext();

        var logMock =
            new Mock<ICreateSystemLog>();

        var emailMock =
            new Mock<IPasswordResetEmailService>();

        var service = new PasswordServices(
            context,
            logMock.Object,
            emailMock.Object,
            CreateFrontendSettings());

        var act = () =>
            service.ResetPasswordAsync(
                "token-invalido",
                "NovaSenha123");

        var exception = await act.Should()
            .ThrowAsync<AppException>();

        exception.Which.Message
            .Should()
            .Be("Token inválido ou expirado.");
    }

    [Fact]
    public async Task ResetPasswordAsync_Should_Throw_When_User_Does_Not_Exist()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var claims =
            new[]
            {
            new Claim("id", "99999"),
            new Claim("email", "fake@email.com")
            };

        var token =
            JwtServices.Create(claims);

        var logMock =
            new Mock<ICreateSystemLog>();

        var emailMock =
            new Mock<IPasswordResetEmailService>();

        var service = new PasswordServices(
            context,
            logMock.Object,
            emailMock.Object,
            CreateFrontendSettings());

        var act = () =>
            service.ResetPasswordAsync(
                token,
                "NovaSenha123");

        var exception = await act.Should()
            .ThrowAsync<AppException>();

        exception.Which.Message
            .Should()
            .Be("Usuário não encontrado.");
    }

    [Fact]
    public async Task ResetPasswordAsync_Should_Update_Password_And_Create_Log()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var originalPassword =
            PasswordHash.Generate("SenhaAntiga");

        var user = new User
        {
            Username = "bruno",
            Email = "bruno@email.com",
            FullName = "Bruno",
            Password = originalPassword
        };

        context.Users.Add(user);

        await context.SaveChangesAsync();

        var token = JwtServices.Create(
        [
            new Claim("id", user.Id.ToString()),
        new Claim("email", user.Email)
        ]);

        var logMock =
            new Mock<ICreateSystemLog>();

        var emailMock =
            new Mock<IPasswordResetEmailService>();

        var service = new PasswordServices(
            context,
            logMock.Object,
            emailMock.Object,
            CreateFrontendSettings());

        await service.ResetPasswordAsync(
            token,
            "NovaSenha123");

        var updatedUser =
            await context.Users
                .FirstAsync(x => x.Id == user.Id);

        PasswordHash.Verify(
            "NovaSenha123",
            updatedUser.Password)
            .Should()
            .BeTrue();

        updatedUser.Password
            .Should()
            .NotBe(originalPassword);

        logMock.Verify(
            x => x.ExecuteAsync(
                It.Is<string>(action =>
                    action.Contains(
                        "alterou a senha")),
                user.Id,
                null,
                null),
            Times.Once);
    }

}
