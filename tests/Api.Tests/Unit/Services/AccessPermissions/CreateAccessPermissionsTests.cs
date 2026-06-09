using Api.Interfaces.Repositories;
using Api.Models;
using Api.Services.AccessPermissions;
using FluentAssertions;
using Moq;

namespace Api.Tests.Unit.Services.AccessPermissions;

public class CreateAccessPermissionsTests
{
    private readonly Mock<IAccessPermissionRepository> _repositoryMock;
    private readonly CreateAccessPermissions _service;

    public CreateAccessPermissionsTests()
    {
        _repositoryMock =
            new Mock<IAccessPermissionRepository>();

        _service =
            new CreateAccessPermissions(
                _repositoryMock.Object);
    }

    #region ExecuteAsync

    [Fact]
    public async Task ExecuteAsync_Should_Create_AccessPermissions()
    {
        // Arrange

        IEnumerable<AccessPermission>? capturedPermissions = null;

        _repositoryMock
            .Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<AccessPermission>>()))
            .Callback<IEnumerable<AccessPermission>>(
                permissions => capturedPermissions = permissions)
            .Returns(Task.CompletedTask);

        var userId = 10;

        var resourceIds =
            new[]
            {
                1,
                2,
                3
            };

        // Act

        await _service.ExecuteAsync(
            userId,
            resourceIds);

        // Assert

        _repositoryMock.Verify(
            x => x.AddRangeAsync(
                It.IsAny<IEnumerable<AccessPermission>>()),
            Times.Once);

        capturedPermissions.Should().NotBeNull();

        var permissions =
            capturedPermissions!.ToList();

        permissions.Should().HaveCount(3);

        permissions.Should()
            .OnlyContain(x => x.UserId == userId);

        permissions.Select(x => x.SystemResourceId)
            .Should()
            .BeEquivalentTo(resourceIds);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Set_CreatedAt_And_UpdatedAt()
    {
        // Arrange

        IEnumerable<AccessPermission>? capturedPermissions = null;

        _repositoryMock
            .Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<AccessPermission>>()))
            .Callback<IEnumerable<AccessPermission>>(
                permissions => capturedPermissions = permissions)
            .Returns(Task.CompletedTask);

        // Act

        await _service.ExecuteAsync(
            1,
            [100]);

        // Assert

        capturedPermissions.Should().NotBeNull();

        var permission =
            capturedPermissions!.Single();

        permission.CreatedAt.Should()
            .BeCloseTo(
                DateTime.UtcNow,
                TimeSpan.FromSeconds(5));

        permission.UpdatedAt.Should()
            .BeCloseTo(
                DateTime.UtcNow,
                TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ExecuteAsync_Should_Create_Empty_Collection_When_No_Permissions_Are_Provided()
    {
        // Arrange

        IEnumerable<AccessPermission>? capturedPermissions = null;

        _repositoryMock
            .Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<AccessPermission>>()))
            .Callback<IEnumerable<AccessPermission>>(
                permissions => capturedPermissions = permissions)
            .Returns(Task.CompletedTask);

        // Act

        await _service.ExecuteAsync(
            1,
            []);

        // Assert

        _repositoryMock.Verify(
            x => x.AddRangeAsync(
                It.IsAny<IEnumerable<AccessPermission>>()),
            Times.Once);

        capturedPermissions.Should().NotBeNull();

        capturedPermissions.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_Should_Propagate_Exception_From_Repository()
    {
        // Arrange

        _repositoryMock
            .Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<AccessPermission>>()))
            .ThrowsAsync(
                new Exception("Database error"));

        // Act

        Func<Task> act =
            () => _service.ExecuteAsync(
                1,
                [1]);

        // Assert

        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage("Database error");
    }

    #endregion
}