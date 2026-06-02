using Api.Auditing;
using Api.Dtos;
using Api.Interfaces.Repositories;
using Api.Interfaces.Auditing.Services;
using Api.Middlewares;
using Api.Services.Users;
using FluentAssertions;
using Moq;

namespace Api.Tests.Unit.Services.Users;

public class DeleteUserTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ICreateSystemLog> _systemLogMock;

    private readonly DeleteUser _service;

    public DeleteUserTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _systemLogMock = new Mock<ICreateSystemLog>();

        _service = new DeleteUser(
            _userRepositoryMock.Object,
            _systemLogMock.Object
        );
    }

    #region ExecuteAsync

    [Fact]
    public async Task ExecuteAsync_ShouldDeleteUser_WhenIdIsValid()
    {
        // Arrange

        const int userId = 1;

        _userRepositoryMock
            .Setup(r => r.SoftDeleteAsync(userId))
            .ReturnsAsync(true);

        // Act

        var result = await _service.ExecuteAsync(userId);

        // Assert

        result.Should().BeTrue();

        _userRepositoryMock.Verify(
            r => r.SoftDeleteAsync(userId),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCreateSystemLog_WhenDeleteSucceeds()
    {
        // Arrange

        const int userId = 1;

        _userRepositoryMock
            .Setup(r => r.SoftDeleteAsync(userId))
            .ReturnsAsync(true);

        // Act

        await _service.ExecuteAsync(userId);

        // Assert

        _systemLogMock.Verify(
            s => s.ExecuteAsync(
                SystemLogActionFactory.Delete("User", userId),
                null,
                null,
                null),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotCreateSystemLog_WhenDeleteFails()
    {
        // Arrange

        const int userId = 1;

        _userRepositoryMock
            .Setup(r => r.SoftDeleteAsync(userId))
            .ReturnsAsync(false);

        // Act

        var result = await _service.ExecuteAsync(userId);

        // Assert

        result.Should().BeFalse();

        _systemLogMock.Verify(
            s => s.ExecuteAsync(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string?>(),
                It.IsAny<SystemLogDataDto?>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenIdIsZero()
    {
        // Act

        Func<Task> act = () => _service.ExecuteAsync(0);

        // Assert

        await act.Should()
            .ThrowAsync<AppException>()
            .WithMessage("Id inválido.");

        _userRepositoryMock.Verify(
            r => r.SoftDeleteAsync(It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenIdIsNegative()
    {
        // Act

        Func<Task> act = () => _service.ExecuteAsync(-1);

        // Assert

        await act.Should()
            .ThrowAsync<AppException>()
            .WithMessage("Id inválido.");

        _userRepositoryMock.Verify(
            r => r.SoftDeleteAsync(It.IsAny<int>()),
            Times.Never);
    }

    #endregion
}