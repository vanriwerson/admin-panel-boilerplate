using Api.Auditing;
using Api.Auditing.Services;
using Api.Dtos;
using Api.Interfaces.Repositories;
using Api.Models;
using Api.Security.Jwt;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace Api.Tests.Unit.Auditing.Services;

public class CreateSystemLogTests
{
    private readonly Mock<ISystemLogRepository> _repositoryMock;

    private readonly CurrentUserContext _currentUser;

    private readonly CreateSystemLog _service;

    public CreateSystemLogTests()
    {
        _repositoryMock =
            new Mock<ISystemLogRepository>();

        var claims =
            new List<Claim>
            {
                new("id", "10"),
                new("username", "bruno")
            };

        var principal =
            new ClaimsPrincipal(
                new ClaimsIdentity(
                    claims,
                    "Test"));

        var httpContext =
            new DefaultHttpContext();

        httpContext.User = principal;

        httpContext.Connection.RemoteIpAddress =
            System.Net.IPAddress.Parse(
                "127.0.0.1");

        var accessor =
            new HttpContextAccessor
            {
                HttpContext = httpContext
            };

        _currentUser =
            new CurrentUserContext(
                accessor);

        _service =
            new CreateSystemLog(
                _repositoryMock.Object,
                _currentUser);
    }

    #region ExecuteAsync

    [Fact]
    public async Task ExecuteAsync_Should_Create_Log_Using_Current_User()
    {
        // Arrange

        SystemLog? capturedLog = null;

        _repositoryMock
            .Setup(x =>
                x.CreateAsync(
                    It.IsAny<SystemLog>()))
            .Callback<SystemLog>(
                log => capturedLog = log)
            .Returns(Task.CompletedTask);

        // Act

        await _service.ExecuteAsync(
            "test action");

        // Assert

        capturedLog.Should().NotBeNull();

        capturedLog!.UserId.Should().Be(10);

        capturedLog.GeneratedBy.Should().Be(
            "bruno");

        capturedLog.Action.Should().Be(
            "test action");

        capturedLog.IpAddress.Should().Be(
            "127.0.0.1");

        capturedLog.CreatedAt.Should()
            .BeCloseTo(
                DateTime.UtcNow,
                TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ExecuteAsync_Should_Use_Provided_UserId_And_GeneratedBy()
    {
        // Arrange

        SystemLog? capturedLog = null;

        _repositoryMock
            .Setup(x =>
                x.CreateAsync(
                    It.IsAny<SystemLog>()))
            .Callback<SystemLog>(
                log => capturedLog = log)
            .Returns(Task.CompletedTask);

        // Act

        await _service.ExecuteAsync(
            action: "custom action",
            userId: 99,
            generatedBy: "admin");

        // Assert

        capturedLog.Should().NotBeNull();

        capturedLog!.UserId.Should().Be(99);

        capturedLog.GeneratedBy.Should().Be(
            "admin");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Serialize_Data()
    {
        // Arrange

        SystemLog? capturedLog = null;

        _repositoryMock
            .Setup(x =>
                x.CreateAsync(
                    It.IsAny<SystemLog>()))
            .Callback<SystemLog>(
                log => capturedLog = log)
            .Returns(Task.CompletedTask);

        var data =
            new SystemLogDataDto
            {
                Type = "create",
                Created = new
                {
                    Id = 1,
                    Name = "Bruno"
                }
            };

        // Act

        await _service.ExecuteAsync(
            action: "create user",
            data: data);

        // Assert

        capturedLog.Should().NotBeNull();

        capturedLog!.Data.Should()
            .NotBeNullOrWhiteSpace();

        var deserialized =
            SystemLogDataSerializer
                .Deserialize(
                    capturedLog.Data);

        deserialized.Should().NotBeNull();

        deserialized!.Type.Should()
            .Be("create");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Not_Set_Data_When_Null()
    {
        // Arrange

        SystemLog? capturedLog = null;

        _repositoryMock
            .Setup(x =>
                x.CreateAsync(
                    It.IsAny<SystemLog>()))
            .Callback<SystemLog>(
                log => capturedLog = log)
            .Returns(Task.CompletedTask);

        // Act

        await _service.ExecuteAsync(
            "action");

        // Assert

        capturedLog.Should().NotBeNull();

        capturedLog!.Data.Should().BeNull();
    }

    [Fact]
    public async Task ExecuteAsync_Should_Call_Repository()
    {
        // Act

        await _service.ExecuteAsync(
            "action");

        // Assert

        _repositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<SystemLog>()),
            Times.Once);
    }

    #endregion
}