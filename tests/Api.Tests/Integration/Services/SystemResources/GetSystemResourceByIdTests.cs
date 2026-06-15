using Api.Models;
using Api.Repositories;
using Api.Services.SystemResources;
using Api.Tests.Integration.Infrastructure;
using FluentAssertions;

namespace Api.Tests.Integration.Services.SystemResources;

[Collection("PostgreSql")]
public class GetSystemResourceByIdTests
{
    private readonly PostgreSqlTestFixture _fixture;

    public GetSystemResourceByIdTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Resource()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var resource =
            new SystemResource
            {
                Name = "USERS",
                ExhibitionName = "Usuários"
            };

        context.SystemResources.Add(resource);

        await context.SaveChangesAsync();

        var service =
            new GetSystemResourceById(
                new SystemResourceRepository(context));

        var result =
            await service.ExecuteAsync(resource.Id);

        result.Id.Should().Be(resource.Id);
        result.Name.Should().Be("USERS");
        result.ExhibitionName.Should().Be("Usuários");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_NotFound()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var service =
            new GetSystemResourceById(
                new SystemResourceRepository(context));

        var act =
            () => service.ExecuteAsync(999);

        await act.Should()
            .ThrowAsync<Api.Middlewares.AppException>()
            .WithMessage("Recurso não encontrado.");
    }
}