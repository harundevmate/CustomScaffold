using Api.Helper;
using AutoMapper;
using Shared;
using Shared.Helper;
using System.Data.Common;
using System.Net;
using System.Text.Json;

namespace Api.Middleware
{

    public class MiddlewareHandling
    {
        private readonly RequestDelegate _next;

        public MiddlewareHandling(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IWebHostEnvironment env)
        {
            try
            {
                await _next(context);
            }
            catch(Exception e)
            {
                await HandleExceptionAsync(context,e,env);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, IWebHostEnvironment env)
        {
            HttpStatusCode status = HttpStatusCode.InternalServerError;
            string message = exception.Message;
            var stackTrace = String.Empty;

            var exceptionType = exception.GetType();
            if(exceptionType == typeof(AutoMapperMappingException) || exceptionType == typeof(ArgumentNullException)
                || exceptionType == typeof(ArgumentOutOfRangeException) || exceptionType == typeof(ArithmeticException)
                || exceptionType == typeof(DivideByZeroException) 
                || exceptionType == typeof(DbException) || exceptionType == typeof(InvalidOperationException) )
            {
                status = HttpStatusCode.BadRequest;
                message = Constant.Message.BadRequest;
            }
            else if(exceptionType == typeof(ArgumentException) || exceptionType == typeof(ApiException))
            {
                status = HttpStatusCode.Conflict;
                message = Constant.Message.Conflict;
            }

            if (env.EnvironmentName == "Development")
                stackTrace = exception.StackTrace;
            var result = JsonSerializer.Serialize(new ApiResult { error = exception.Message, status = ((int)status).ToString(),message = message});
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            return context.Response.WriteAsync(result);
        }
    }
}
