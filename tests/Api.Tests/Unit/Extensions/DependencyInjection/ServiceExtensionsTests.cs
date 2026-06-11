using Api.Auditing.Services;
using Api.Extensions.DependencyInjection;
using Api.Interfaces.Auditing.Services;
using Api.Services.Users;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Api.Tests.Unit.Extensions.DependencyInjection;

public class ServiceExtensionsTests
{
    [Fact]
    public void AddApplicationServices_Should_Register_GetUserById()
    {
        var services =
            new ServiceCollection();

        var logger =
            Mock.Of<ILogger>();

        services.AddApplicationServices(logger);

        var descriptor =
            services.FirstOrDefault(s =>
                s.ServiceType == typeof(GetUserById));

        descriptor.Should().NotBeNull();
    }

    [Fact]
    public void AddApplicationServices_Should_Register_DeleteUser()
    {
        var services =
            new ServiceCollection();

        var logger =
            Mock.Of<ILogger>();

        services.AddApplicationServices(logger);

        var descriptor =
            services.FirstOrDefault(s =>
                s.ServiceType == typeof(DeleteUser));

        descriptor.Should().NotBeNull();
    }

    [Fact]
    public void AddApplicationServices_Should_Register_CreateSystemLog_Interface()
    {
        var services =
            new ServiceCollection();

        var logger =
            Mock.Of<ILogger>();

        services.AddApplicationServices(logger);

        var descriptor =
            services.FirstOrDefault(s =>
                s.ServiceType == typeof(ICreateSystemLog));

        descriptor.Should().NotBeNull();
    }

    [Fact]
    public void AddApplicationServices_Should_Register_GetAllSystemLogs()
    {
        var services =
            new ServiceCollection();

        var logger =
            Mock.Of<ILogger>();

        services.AddApplicationServices(logger);

        var descriptor =
            services.FirstOrDefault(s =>
                s.ServiceType == typeof(GetAllSystemLogs));

        descriptor.Should().NotBeNull();
    }
}