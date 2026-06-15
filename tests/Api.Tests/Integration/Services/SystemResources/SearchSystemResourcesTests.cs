using Api.Models;
using Api.Repositories;
using Api.Services.SystemResources;
using Api.Tests.Integration.Infrastructure;
using FluentAssertions;

namespace Api.Tests.Integration.Services.SystemResources;

[Collection("PostgreSql")]
public class SearchSystemResourcesTests
{
    private readonly PostgreSqlTestFixture _fixture;

    public SearchSystemResourcesTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Matching_Resources()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.SystemResources.AddRange(
            new SystemResource
            {
                Name = "USERS",
                ExhibitionName = "Usuários"
            },
            new SystemResource
            {
                Name = "REPORTS",
                ExhibitionName = "Relatórios"
            });

        await context.SaveChangesAsync();

        var service =
            new SearchSystemResources(
                new SystemResourceRepository(context));

        var result =
            await service.ExecuteAsync("USER");

        result.TotalItems.Should().Be(1);

        result.Data.Single().Name
            .Should().Be("USERS");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Search_By_ExhibitionName()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.SystemResources.Add(
            new SystemResource
            {
                Name = "USERS",
                ExhibitionName = "Cadastro de Usuários"
            });

        await context.SaveChangesAsync();

        var service =
            new SearchSystemResources(
                new SystemResourceRepository(context));

        var result =
            await service.ExecuteAsync("Cadastro");

        result.TotalItems.Should().Be(1);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Empty_When_NotFound()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var service =
            new SearchSystemResources(
                new SystemResourceRepository(context));

        var result =
            await service.ExecuteAsync("INEXISTENTE");

        result.TotalItems.Should().Be(0);

        result.Data.Should().BeEmpty();
    }
}