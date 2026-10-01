using System.Text.Json;
using EasyRoster.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace EasyRoster.Api.Tests;

public sealed class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task UnhandledException_ReturnsProblemDetailsWithoutExceptionMessage()
    {
        var sut = new ExceptionHandlingMiddleware(
            _ => throw new InvalidOperationException("database password should never leak"),
            NullLogger<ExceptionHandlingMiddleware>.Instance);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await sut.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        context.Response.Body.Position = 0;
        using var body = await JsonDocument.ParseAsync(context.Response.Body);
        Assert.Equal("An unexpected error occurred", body.RootElement.GetProperty("title").GetString());
        Assert.True(body.RootElement.TryGetProperty("traceId", out _));
        Assert.DoesNotContain("database password", body.RootElement.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}
