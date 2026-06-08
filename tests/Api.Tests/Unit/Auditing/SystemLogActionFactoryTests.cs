using Api.Auditing;
using FluentAssertions;

namespace Api.Tests.Unit.Auditing;

public class SystemLogActionFactoryTests
{
    #region Create

    [Fact]
    public void Create_Should_Return_Expected_Text()
    {
        var result =
            SystemLogActionFactory.Create(
                "User",
                1);

        result.Should()
            .Be("create User id: 1");
    }

    #endregion

    #region Update

    [Fact]
    public void Update_Should_Return_Expected_Text()
    {
        var result =
            SystemLogActionFactory.Update(
                "User",
                5);

        result.Should()
            .Be("update User id: 5");
    }

    #endregion

    #region Delete

    [Fact]
    public void Delete_Should_Return_Expected_Text()
    {
        var result =
            SystemLogActionFactory.Delete(
                "User",
                8);

        result.Should()
            .Be("delete User id: 8");
    }

    #endregion

    #region Login

    [Fact]
    public void Login_Should_Return_Expected_Text()
    {
        var result =
            SystemLogActionFactory.Login(
                "bruno");

        result.Should()
            .Be("user bruno fez login no sistema");
    }

    #endregion

    #region ExternalLogin

    [Fact]
    public void ExternalLogin_Should_Return_Expected_Text()
    {
        var result =
            SystemLogActionFactory.ExternalLogin(
                "bruno");

        result.Should()
            .Be(
                "user bruno fez login por redirecionamento no sistema");
    }

    #endregion

    #region Logout

    [Fact]
    public void Logout_Should_Return_Expected_Text()
    {
        var result =
            SystemLogActionFactory.Logout(
                "bruno");

        result.Should()
            .Be("user bruno fez logout do sistema");
    }

    #endregion

    #region NewPasswordRequest

    [Fact]
    public void NewPasswordRequest_Should_Return_Expected_Text()
    {
        var result =
            SystemLogActionFactory.NewPasswordRequest(
                "bruno");

        result.Should()
            .Be(
                "user bruno solicitou reset de senha");
    }

    #endregion

    #region PasswordReset

    [Fact]
    public void PasswordReset_Should_Return_Expected_Text()
    {
        var result =
            SystemLogActionFactory.PasswordReset(
                "bruno");

        result.Should()
            .Be(
                "user bruno alterou a senha");
    }

    #endregion

    #region TokenRefreshed

    [Fact]
    public void TokenRefreshed_Should_Return_Expected_Text()
    {
        var result =
            SystemLogActionFactory.TokenRefreshed(
                "bruno");

        result.Should()
            .Be(
                "user bruno renovou o token");
    }

    #endregion
}