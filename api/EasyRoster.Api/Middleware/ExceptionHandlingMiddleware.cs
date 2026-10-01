using Microsoft.AspNetCore.Mvc;
namespace EasyRoster.Api.Middleware;
public sealed class ExceptionHandlingMiddleware(RequestDelegate next,ILogger<ExceptionHandlingMiddleware> logger) {
 public async Task InvokeAsync(HttpContext context) { try { await next(context); } catch(Exception ex) { logger.LogError(ex,"Unhandled exception. TraceId: {TraceId}",context.TraceIdentifier); context.Response.StatusCode=500; await context.Response.WriteAsJsonAsync(new ProblemDetails{Status=500,Title="An unexpected error occurred",Extensions={{"traceId",context.TraceIdentifier}}}); } }
}
