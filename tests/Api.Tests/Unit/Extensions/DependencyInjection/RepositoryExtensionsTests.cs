using Api.Extensions.DependencyInjection;
using Api.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Api.Tests.Unit.Extensions.DependencyInjection;

public class RepositoryExtensionsTests
{
    [Fact]
    public void AddRepositories_Should_Register_UserRepository()
    {
        var services = new ServiceCollection();

        var logger =
            Mock.Of<ILogger>();

        services.AddRepositories(logger);

        var descriptor =
            services.FirstOrDefault(s =>
                s.ServiceType == typeof(IUserRepository));

        descriptor.Should().NotBeNull();
    }

    [Fact]
    public void AddRepositories_Should_Register_SystemLogRepository()
    {
        var services = new ServiceCollection();

        var logger =
            Mock.Of<ILogger>();

        services.AddRepositories(logger);

        var descriptor =
            services.FirstOrDefault(s =>
                s.ServiceType == typeof(ISystemLogRepository));

        descriptor.Should().NotBeNull();
    }

    [Fact]
    public void AddRepositories_Should_Register_AccessPermissionRepository()
    {
        var services = new ServiceCollection();

        var logger =
            Mock.Of<ILogger>();

        services.AddRepositories(logger);

        var descriptor =
            services.FirstOrDefault(s =>
                s.ServiceType == typeof(IAccessPermissionRepository));

        descriptor.Should().NotBeNull();
    }
}