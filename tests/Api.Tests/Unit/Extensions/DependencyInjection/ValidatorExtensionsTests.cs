using Api.Extensions.DependencyInjection;
using Api.Interfaces.Validators;
using Api.Validations;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Api.Tests.Unit.Extensions.DependencyInjection;

public class ValidatorExtensionsTests
{
    [Fact]
    public void AddValidators_Should_Register_UserValidator()
    {
        var services = new ServiceCollection();

        var logger =
            Mock.Of<ILogger>();

        services.AddValidators(logger);

        var descriptor =
            services.FirstOrDefault(s =>
                s.ServiceType == typeof(IUserValidator));

        descriptor.Should().NotBeNull();

        descriptor!.ImplementationType
            .Should()
            .Be(typeof(UserValidator));
    }

    [Fact]
    public void AddValidators_Should_Register_Validators()
    {
        var services = new ServiceCollection();

        var logger =
            Mock.Of<ILogger>();

        services.AddValidators(logger);

        services.Should()
            .Contain(s =>
                s.ImplementationType != null &&
                s.ImplementationType.Name.EndsWith("Validator"));
    }
}