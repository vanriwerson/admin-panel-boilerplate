using Api.Middlewares;
using FluentAssertions;

namespace Api.Tests.Unit.Middlewares;

public class GuardTests
{
    [Fact]
    public void AgainstNull_ShouldThrow_WhenValueIsNull()
    {
        Action act = () =>
            Guard.AgainstNull(null, "Valor obrigatório");

        act.Should()
            .Throw<AppException>()
            .WithMessage("Valor obrigatório");
    }

    [Fact]
    public void AgainstNullOrEmpty_ShouldThrow_WhenValueIsEmpty()
    {
        Action act = () =>
            Guard.AgainstNullOrEmpty("", "Username");

        act.Should()
            .Throw<AppException>()
            .WithMessage("'Username' é obrigatório.");
    }

    [Fact]
    public void AgainstNullOrEmpty_ShouldThrow_WhenValueIsWhitespace()
    {
        Action act = () =>
            Guard.AgainstNullOrEmpty("   ", "Username");

        act.Should()
            .Throw<AppException>()
            .WithMessage("'Username' é obrigatório.");
    }

    [Fact]
    public void AgainstNullOrEmpty_ShouldNotThrow_WhenValueIsValid()
    {
        Action act = () =>
            Guard.AgainstNullOrEmpty("Bruno", "Username");

        act.Should().NotThrow();
    }

    [Fact]
    public void AgainstNonPositiveInt_ShouldThrow_WhenValueIsZero()
    {
        Action act = () =>
            Guard.AgainstNonPositiveInt(0);

        act.Should()
            .Throw<AppException>()
            .WithMessage("Id inválido.");
    }

    [Fact]
    public void AgainstNonPositiveInt_ShouldThrow_WhenValueIsNegative()
    {
        Action act = () =>
            Guard.AgainstNonPositiveInt(-1);

        act.Should()
            .Throw<AppException>()
            .WithMessage("Id inválido.");
    }

    [Fact]
    public void AgainstNonPositiveInt_ShouldNotThrow_WhenValueIsPositive()
    {
        Action act = () =>
            Guard.AgainstNonPositiveInt(1);

        act.Should().NotThrow();
    }

    [Fact]
    public void AgainstMismatchedIds_ShouldThrow_WhenIdsAreDifferent()
    {
        Action act = () =>
            Guard.AgainstMismatchedIds(1, 2);

        act.Should()
            .Throw<AppException>()
            .WithMessage("Id da rota difere do id do payload.");
    }

    [Fact]
    public void AgainstMismatchedIds_ShouldNotThrow_WhenIdsAreEqual()
    {
        Action act = () =>
            Guard.AgainstMismatchedIds(1, 1);

        act.Should().NotThrow();
    }
}