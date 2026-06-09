using Api.Models;
using Api.Repositories;
using Api.Tests.Integration.Infrastructure;
using FluentAssertions;

namespace Api.Tests.Integration.Repositories;

[Collection("PostgreSql")]
public class SystemResourceRepositoryTests
{
    private readonly PostgreSqlTestFixture _fixture;

    public SystemResourceRepositoryTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateAsync_Should_Add_Resource()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var repository =
            new SystemResourceRepository(context);

        var resource =
            new SystemResource
            {
                Name = "USERS",
                ExhibitionName = "Usuários"
            };

        await repository.CreateAsync(resource);

        await context.SaveChangesAsync();

        resource.Id.Should().BeGreaterThan(0);

        context.SystemResources
            .Should()
            .ContainSingle(x => x.Name == "USERS");
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Resource()
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

        var repository =
            new SystemResourceRepository(context);

        resource.ExhibitionName =
            "Usuários Atualizado";

        await repository.UpdateAsync(resource);

        await context.SaveChangesAsync();

        var updated =
            await repository.GetByIdAsync(resource.Id);

        updated!.ExhibitionName
            .Should()
            .Be("Usuários Atualizado");
    }

    [Fact]
    public async Task SoftDeleteAsync_Should_Deactivate_Resource()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var resource =
            new SystemResource
            {
                Name = "REPORTS",
                ExhibitionName = "Relatórios"
            };

        context.SystemResources.Add(resource);

        await context.SaveChangesAsync();

        var repository =
            new SystemResourceRepository(context);

        var result =
            await repository.SoftDeleteAsync(resource.Id);

        result.Should().BeTrue();

        var entity =
            context.SystemResources.First(x =>
                x.Id == resource.Id);

        entity.Active.Should().BeFalse();
    }

    [Fact]
    public async Task SoftDeleteAsync_Should_Return_False_When_Not_Found()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var repository =
            new SystemResourceRepository(context);

        var result =
            await repository.SoftDeleteAsync(9999);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Active_Resource()
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

        var repository =
            new SystemResourceRepository(context);

        var result =
            await repository.GetByIdAsync(resource.Id);

        result.Should().NotBeNull();

        result!.Name.Should().Be("USERS");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Inactive()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var resource =
            new SystemResource
            {
                Name = "ROOT",
                ExhibitionName = "Administrador",
                Active = false
            };

        context.SystemResources.Add(resource);

        await context.SaveChangesAsync();

        var repository =
            new SystemResourceRepository(context);

        var result =
            await repository.GetByIdAsync(resource.Id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Only_Active_Resources()
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
                Name = "ROOT",
                ExhibitionName = "Administrador",
                Active = false
            });

        await context.SaveChangesAsync();

        var repository =
            new SystemResourceRepository(context);

        var result =
            await repository.GetAllAsync(1, 20);

        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task SearchAsync_Should_Filter_By_Name()
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

        var repository =
            new SystemResourceRepository(context);

        var result =
            await repository.SearchAsync(
                "USER",
                1,
                20);

        result.Data.Should().ContainSingle();

        result.Data.First().Name
            .Should()
            .Be("USERS");
    }

    [Fact]
    public async Task ExistsByNameAsync_Should_Return_True_When_Exists()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.SystemResources.Add(
            new SystemResource
            {
                Name = "USERS",
                ExhibitionName = "Usuários"
            });

        await context.SaveChangesAsync();

        var repository =
            new SystemResourceRepository(context);

        var result =
            await repository.ExistsByNameAsync("USERS");

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByNameAsync_Should_Return_False_When_Not_Exists()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var repository =
            new SystemResourceRepository(context);

        var result =
            await repository.ExistsByNameAsync("NOT_FOUND");

        result.Should().BeFalse();
    }


}
