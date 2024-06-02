using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public static class StatusCodesExtension
{
    [DefaultStatusCode(DefaultStatusCode)]
    public class FailedDependencyResult : ObjectResult
    {
        private const int DefaultStatusCode = StatusCodes.Status424FailedDependency;

        public FailedDependencyResult(object? value) : base(value) => StatusCode = DefaultStatusCode;
    }

    public static FailedDependencyResult FailedDependency(this ControllerBase _, object? value = null)
        => new FailedDependencyResult(value);
}
