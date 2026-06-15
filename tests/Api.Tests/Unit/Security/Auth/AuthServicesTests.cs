using Api.Auditing;
using Api.Dtos;
using Api.Interfaces.Auditing.Services;
using Api.Interfaces.Security.Auth;
using Api.Interfaces.Security.RefreshTokens;
using Api.Models;
using Api.Security.Auth;
using Api.Security.Passwords;
using FluentAssertions;
using Moq;

namespace Api.Tests.Unit.Security.Auth;

public class AuthServicesTests
{
    private readonly Mock<IAuthUserResolver> _resolverMock;
    private readonly Mock<ILoginResponseFactory> _loginResponseFactoryMock;
    private readonly Mock<IRefreshTokenServices> _refreshTokenServiceMock;
    private readonly Mock<ICreateSystemLog> _logMock;

    private readonly AuthServices _service;

    public AuthServicesTests()
    {
        _resolverMock =
            new Mock<IAuthUserResolver>();

        _loginResponseFactoryMock =
            new Mock<ILoginResponseFactory>();

        _refreshTokenServiceMock =
            new Mock<IRefreshTokenServices>();

        _logMock =
            new Mock<ICreateSystemLog>();

        _service =
            new AuthServices(
                _resolverMock.Object,
                _loginResponseFactoryMock.Object,
                _refreshTokenServiceMock.Object,
                _logMock.Object
            );
    }

    #region LoginAsync

    [Fact]
    public async Task LoginAsync_Should_Return_Response_When_Credentials_Are_Valid()
    {
        // Arrange

        var password = "123456";

        var user =
            new User
            {
                Id = 1,
                Username = "bruno",
                FullName = "Bruno Silva",
                Password = PasswordHash.Generate(password)
            };

        var response =
            new LoginResponseDto
            {
                Token = "jwt",
                RefreshToken = "refresh"
            };

        _resolverMock
            .Setup(x =>
                x.FindByIdentifierAsync("bruno"))
            .ReturnsAsync(user);

        _loginResponseFactoryMock
            .Setup(x =>
                x.CreateResponseAsync(user))
            .ReturnsAsync(response);

        // Act

        var result =
            await _service.LoginAsync(
                "bruno",
                password);

        // Assert

        result.Should().BeSameAs(response);

        _logMock.Verify(
            x => x.ExecuteAsync(
                SystemLogActionFactory.Login(
                    user.Username),
                user.Id,
                user.Username,
                null),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_Should_Return_Null_When_User_Does_Not_Exist()
    {
        // Arrange

        _resolverMock
            .Setup(x =>
                x.FindByIdentifierAsync(
                    It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        // Act

        var result =
            await _service.LoginAsync(
                "bruno",
                "123456");

        // Assert

        result.Should().BeNull();

        _loginResponseFactoryMock.Verify(
            x => x.CreateResponseAsync(
                It.IsAny<User>()),
            Times.Never);

        _logMock.Verify(
            x => x.ExecuteAsync(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string?>(),
                It.IsAny<SystemLogDataDto>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_Should_Return_Null_When_Password_Is_Invalid()
    {
        // Arrange

        var user =
            new User
            {
                Username = "bruno",
                Password = PasswordHash.Generate(
                    "senha-correta")
            };

        _resolverMock
            .Setup(x =>
                x.FindByIdentifierAsync("bruno"))
            .ReturnsAsync(user);

        // Act

        var result =
            await _service.LoginAsync(
                "bruno",
                "senha-incorreta");

        // Assert

        result.Should().BeNull();

        _loginResponseFactoryMock.Verify(
            x => x.CreateResponseAsync(
                It.IsAny<User>()),
            Times.Never);
    }

    #endregion

    #region LogoutAsync

    [Fact]
    public async Task LogoutAsync_Should_Revoke_All_User_Tokens()
    {
        // Act

        await _service.LogoutAsync(
            1,
            "bruno");

        // Assert

        _refreshTokenServiceMock.Verify(
            x => x.RevokeAllForUserAsync(1),
            Times.Once);

        _logMock.Verify(
            x => x.ExecuteAsync(
                SystemLogActionFactory.Logout(
                    "bruno"),
                1,
                "bruno",
                null),
            Times.Once);
    }

    #endregion

    #region RefreshAsync

    [Fact]
    public async Task RefreshAsync_Should_Return_New_Tokens()
    {
        // Arrange

        var user =
            new User
            {
                Id = 1,
                Username = "bruno"
            };

        var storedToken =
            new RefreshToken
            {
                User = user
            };

        var loginResponse =
            new LoginResponseDto
            {
                Token = "new-jwt",
                RefreshToken = "new-refresh"
            };

        _refreshTokenServiceMock
            .Setup(x =>
                x.ValidateAsync("token"))
            .ReturnsAsync(storedToken);

        _loginResponseFactoryMock
            .Setup(x =>
                x.CreateResponseAsync(user))
            .ReturnsAsync(loginResponse);

        // Act

        var result =
            await _service.RefreshAsync(
                "token");

        // Assert

        result.Should().NotBeNull();

        result!.Token.Should().Be(
            "new-jwt");

        result.RefreshToken.Should().Be(
            "new-refresh");

        _refreshTokenServiceMock.Verify(
            x => x.RevokeAsync(storedToken),
            Times.Once);

        _logMock.Verify(
            x => x.ExecuteAsync(
                SystemLogActionFactory.TokenRefreshed(
                    user.Username),
                user.Id,
                user.Username,
                null),
            Times.Once);
    }

    [Fact]
    public async Task RefreshAsync_Should_Return_Null_When_Token_Is_Invalid()
    {
        // Arrange

        _refreshTokenServiceMock
            .Setup(x =>
                x.ValidateAsync("token"))
            .ReturnsAsync((RefreshToken?)null);

        // Act

        var result =
            await _service.RefreshAsync(
                "token");

        // Assert

        result.Should().BeNull();

        _loginResponseFactoryMock.Verify(
            x => x.CreateResponseAsync(
                It.IsAny<User>()),
            Times.Never);

        _refreshTokenServiceMock.Verify(
            x => x.RevokeAsync(
                It.IsAny<RefreshToken>()),
            Times.Never);
    }

    #endregion
}