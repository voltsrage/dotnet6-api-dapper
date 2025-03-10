using Dapper.API.Exceptions;
using Dapper.API.Helpers;
using Dapper.API.Models;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace Dapper.API.Middlewares
{
    public class CustomExceptionsHandlerMiddleware : IMiddleware
    {
        private readonly ILogger<CustomExceptionsHandlerMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public CustomExceptionsHandlerMiddleware(
            ILogger<CustomExceptionsHandlerMiddleware> logger, 
            IHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private async Task HandleException(HttpContext context, Exception ex)
        {
            var errorResponse = CreateErrorResponse(ex, context);
            context.Response.StatusCode = DetermineStatusCode(ex);
            context.Response.ContentType = "application/json";

            // Log the error with structured logging
            LogError(ex, context);

            var returnError = new Response<object>
            {
                Content = errorResponse,
                IsSuccess = false,
                StatusCode = context.Response.StatusCode,
                ErrorMessage = ex.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(returnError, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));

        }

        private void LogError(Exception ex, HttpContext context)
        {
            var logLevel = ex switch
            {
                ValidationException => LogLevel.Information,
                AuthenticationException => LogLevel.Warning,
                _ => LogLevel.Error
            };

            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["TraceId"] = context.TraceIdentifier,
                ["Path"] = context.Request.Path,
                ["Method"] = context.Request.Method,
                ["ClientIP"] = context.Connection.RemoteIpAddress,
                ["UserAgent"] = context.Request.Headers["User-Agent"].ToString()
            }))
            {
                if (ex is BaseException authException)
                {
                    _logger.Log(logLevel, ex,
                        "Error occurred in {Component}.{Function} - {ErrorCode}: {Message}",
                        authException.Component,
                        authException.Function,
                        authException.ErrorCode,
                        authException.Message);
                }
                else
                {
                    _logger.Log(logLevel, ex,
                        "Unhandled exception: {Message}",
                        ex.Message);
                }
            }
        }

        private int DetermineStatusCode(Exception exception) => exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            PaginationDatabaseException => StatusCodes.Status500InternalServerError,
            PaginationException => StatusCodes.Status400BadRequest,
            AuthenticationException authException => (int)authException.ErrorType.StatusCode,
            RepositoryException => StatusCodes.Status500InternalServerError,   
            _ => StatusCodes.Status500InternalServerError
        };

        private object CreateErrorResponse(Exception ex, HttpContext context)
        {
            var response = new ErrorResponse
            {
                TraceId = Activity.Current?.Id ?? context?.TraceIdentifier,
                Timestamp = DateTimeHelper.GetCurrentTimestamp(),
            };

            switch (ex)
            {
                case ValidationException validationException:
                    response.Error = new ErrorDetail
                    {
                        Code = validationException.ErrorCode,
                        Message = "Validation failed",
                        Component = validationException.Component,
                        Function = validationException.Function,
                        ValidationErrors = validationException.Errors
                    };
                    break;

                case BaseException authException:
                    response.Error = new ErrorDetail
                    {
                        Code = authException.ErrorCode,
                        Message = authException.Message,
                        Component = authException.Component,
                        Function = authException.Function,
                        AdditionalData = authException.AdditionalData
                    };
                    break;


                default:
                    response.Error = new ErrorDetail
                    {
                        Code = "INTERNAL_ERROR",
                        Message = _environment.IsDevelopment()
                            ? ex.Message
                            : "An unexpected error occurred."
                    };
                    break;
            }

            return response;
        }
    }
}
