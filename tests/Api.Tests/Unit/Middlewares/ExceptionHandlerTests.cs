using System.Text.Json;
using Api.Middlewares;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Api.Tests.Unit.Middlewares;

public class ExceptionHandlerTests
{
    [Fact]
    public async Task InvokeAsync_Should_Call_Next_When_No_Exception()
    {
        // Arrange
        var nextCalled = false;

        RequestDelegate next = context =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var logger =
            new Mock<ILogger<ExceptionHandler>>();

        var middleware =
            new ExceptionHandler(
                next,
                logger.Object);

        var context =
            new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_Should_Return_AppException_Response()
    {
        // Arrange
        RequestDelegate next = _ =>
            throw new AppException(
                "Erro de negócio",
                StatusCodes.Status400BadRequest);

        var logger =
            new Mock<ILogger<ExceptionHandler>>();

        var middleware =
            new ExceptionHandler(
                next,
                logger.Object);

        var context =
            new DefaultHttpContext();

        context.Response.Body =
            new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode
            .Should()
            .Be(StatusCodes.Status400BadRequest);

        context.Response.ContentType
            .Should()
            .Be("application/json");

        context.Response.Body.Position = 0;

        var response =
            await new StreamReader(
                context.Response.Body)
            .ReadToEndAsync();

        var json =
            JsonDocument.Parse(response);

        json.RootElement
            .GetProperty("error")
            .GetString()
            .Should()
            .Be("Erro de negócio");

        json.RootElement
            .GetProperty("status")
            .GetInt32()
            .Should()
            .Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_Should_Return_InternalServerError_For_Unexpected_Exception()
    {
        // Arrange
        RequestDelegate next = _ =>
            throw new InvalidOperationException(
                "Boom");

        var logger =
            new Mock<ILogger<ExceptionHandler>>();

        var middleware =
            new ExceptionHandler(
                next,
                logger.Object);

        var context =
            new DefaultHttpContext();

        context.Response.Body =
            new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode
            .Should()
            .Be(StatusCodes.Status500InternalServerError);

        context.Response.ContentType
            .Should()
            .Be("application/json");

        context.Response.Body.Position = 0;

        var response =
            await new StreamReader(
                context.Response.Body)
            .ReadToEndAsync();

        var json =
            JsonDocument.Parse(response);

        json.RootElement
            .GetProperty("error")
            .GetString()
            .Should()
            .Be("Ocorreu um erro inesperado.");

        json.RootElement
            .GetProperty("status")
            .GetInt32()
            .Should()
            .Be(StatusCodes.Status500InternalServerError);
    }

}
