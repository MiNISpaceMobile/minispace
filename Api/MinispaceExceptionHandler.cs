using Domain.Services;
using Microsoft.AspNetCore.Diagnostics;

namespace Api;

public class MinispaceExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not MinispaceException)
            return false;

        if (exception is UserUnauthorizedException)
            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
        else
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        httpContext.Response.ContentType = "text/plain";
        await httpContext.Response.WriteAsync(exception.Message, cancellationToken);

        return true;
    }
}
