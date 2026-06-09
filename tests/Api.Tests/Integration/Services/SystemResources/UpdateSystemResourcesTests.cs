using Api.Data;
using Api.Dtos;
using Api.Interfaces.Auditing.Services;
using Api.Interfaces.Repositories;
using Api.Middlewares;
using Api.Models;
using Api.Services.SystemResources;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Api.Tests.Unit.Services.SystemResources;

public class UpdateSystemResourceTests
{
    private readonly Mock<ISystemResourceRepository> _repositoryMock;
    private readonly Mock<ICreateSystemLog> _logMock;
    private readonly Mock<ApiDbContext> _contextMock;

    private readonly UpdateSystemResource _service;

    public UpdateSystemResourceTests()
    {
        _repositoryMock = new Mock<ISystemResourceRepository>();
        _logMock = new Mock<ICreateSystemLog>();

        var options =
            new DbContextOptionsBuilder<ApiDbContext>()
                .Options;

        _contextMock =
            new Mock<ApiDbContext>(options);

        _service = new UpdateSystemResource(
            _repositoryMock.Object,
            _contextMock.Object,
            _logMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Update_Resource()
    {
        // Arrange
        var resource = new SystemResource
        {
            Id = 1,
            Name = "OLD_NAME",
            ExhibitionName = "Old Exhibition"
        };

        var dto = new SystemResourceUpdateDto
        {
            Id = 1,
            Name = "NEW_NAME",
            ExhibitionName = "New Exhibition"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(resource);

        _repositoryMock
            .Setup(r => r.UpdateAsync(resource))
            .ReturnsAsync(resource);

        // Act
        var result =
            await _service.ExecuteAsync(1, dto);

        // Assert
        result.Should().NotBeNull();

        result.Id.Should().Be(1);
        result.Name.Should().Be("NEW_NAME");
        result.ExhibitionName.Should().Be("New Exhibition");

        resource.Name.Should().Be("NEW_NAME");
        resource.ExhibitionName.Should().Be("New Exhibition");

        _repositoryMock.Verify(
            r => r.UpdateAsync(resource),
            Times.Once);

        _logMock.Verify(
            l => l.ExecuteAsync(
                It.Is<string>(s =>
                    s.Contains("update SystemResource")),
                null,
                null,
                It.IsAny<SystemLogDataDto>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_Resource_Not_Found()
    {
        // Arrange
        var dto = new SystemResourceUpdateDto
        {
            Id = 1,
            Name = "NAME",
            ExhibitionName = "EXHIBITION"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((SystemResource?)null);

        // Act
        Func<Task> act =
            () => _service.ExecuteAsync(1, dto);

        // Assert
        await act.Should()
            .ThrowAsync<AppException>()
            .WithMessage("Recurso não encontrado.");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_Ids_Are_Different()
    {
        // Arrange
        var dto = new SystemResourceUpdateDto
        {
            Id = 2,
            Name = "NAME",
            ExhibitionName = "EXHIBITION"
        };

        // Act
        Func<Task> act =
            () => _service.ExecuteAsync(1, dto);

        // Assert
        await act.Should()
            .ThrowAsync<AppException>();
    }

    [Fact]
    public async Task ExecuteAsync_Should_Update_Only_Name()
    {
        // Arrange
        var resource = new SystemResource
        {
            Id = 1,
            Name = "OLD_NAME",
            ExhibitionName = "Old Exhibition"
        };

        var dto = new SystemResourceUpdateDto
        {
            Id = 1,
            Name = "NEW_NAME",
            ExhibitionName = ""
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(resource);

        _repositoryMock
            .Setup(r => r.UpdateAsync(resource))
            .ReturnsAsync(resource);

        // Act
        var result =
            await _service.ExecuteAsync(1, dto);

        // Assert
        result.Name.Should().Be("NEW_NAME");

        result.ExhibitionName
            .Should()
            .Be("Old Exhibition");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Generate_SystemLog_With_Previous_And_Current_State()
    {
        // Arrange
        var resource = new SystemResource
        {
            Id = 1,
            Name = "OLD_NAME",
            ExhibitionName = "Old Exhibition"
        };

        var dto = new SystemResourceUpdateDto
        {
            Id = 1,
            Name = "NEW_NAME",
            ExhibitionName = "New Exhibition"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(resource);

        _repositoryMock
            .Setup(r => r.UpdateAsync(resource))
            .ReturnsAsync(resource);

        // Act
        await _service.ExecuteAsync(1, dto);

        // Assert
        _logMock.Verify(
            l => l.ExecuteAsync(
                It.IsAny<string>(),
                null,
                null,
                It.Is<SystemLogDataDto>(d =>
                    d.Type == "update" &&
                    d.PrevState != null &&
                    d.CurrState != null)),
            Times.Once);
    }
}