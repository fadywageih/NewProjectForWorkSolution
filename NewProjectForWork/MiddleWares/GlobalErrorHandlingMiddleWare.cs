    using Domain.Exceptions;
    using Shared.ErrorModels;
    using System.Net;

    namespace NewProjectForWork.MiddleWares
    {
    public class GlobalErrorHandlingMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlingMiddleWare> _logger;

        public GlobalErrorHandlingMiddleWare(RequestDelegate next, ILogger<GlobalErrorHandlingMiddleWare> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
                if (httpContext.Response.StatusCode == (int)HttpStatusCode.NotFound)
                    await HandleNotFoundApiAsync(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong:{ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleNotFoundApiAsync(HttpContext httpContext)
        {
            httpContext.Response.ContentType = "application/json";
            var response = new ErrorDetails
            {
                StatusCode = httpContext.Response.StatusCode,
                ErrorMessage = $"The API {httpContext.Request.Path} you are trying to reach is not found"
            }.ToString();
            await httpContext.Response.WriteAsync(response);
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.ContentType = "application/json";

            var response = new ErrorDetails
            {
                ErrorMessage = ex.Message,
                StatusCode = (int)HttpStatusCode.InternalServerError // قيمة افتراضية
            };

            // معالجة أنواع الاستثناءات المختلفة
            httpContext.Response.StatusCode = ex switch
            {
                NotFoundException => (int)HttpStatusCode.NotFound,
                UnauthorizedException => (int)HttpStatusCode.Unauthorized, // هذا هو الصحيح!
                Domain.Exceptions.ValidationException => HandleValidationException((Domain.Exceptions.ValidationException)ex, response),
                _ => (int)HttpStatusCode.InternalServerError
            };

            response.StatusCode = httpContext.Response.StatusCode;
            await httpContext.Response.WriteAsync(response.ToString());
        }

        private int HandleValidationException(Domain.Exceptions.ValidationException validationException, ErrorDetails response)
        {
            response.Errors = validationException.Errors;
            return (int)HttpStatusCode.BadRequest;
        }
    }

}
