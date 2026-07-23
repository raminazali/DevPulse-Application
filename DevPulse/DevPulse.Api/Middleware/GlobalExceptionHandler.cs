using DevPulse.Core.Exceptions;
using DevPulse.Core.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace DevPulse.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception,
            "Exception occurred: {Message}",
            exception.Message);

        httpContext.Response.ContentType = "application/problem+json";

        var problemDetails = exception switch
        {
            UnauthorizedException unauth => CreateProblemDetails(
                httpContext,
                StatusCodes.Status401Unauthorized,
                unauth.Message,
                unauth.Title,
                unauth.Code),
            ForbiddenException forbidden => CreateProblemDetails(
                httpContext,
                StatusCodes.Status403Forbidden,
                forbidden.Message,
                forbidden.Title,
                forbidden.Code),
            NotFoundException notFound => CreateProblemDetails(
                httpContext,
                StatusCodes.Status404NotFound,
                notFound.Message,
                notFound.Title,
                notFound.Code),
            ValidationException validation => CreateProblemDetails(
                httpContext,
                StatusCodes.Status400BadRequest,
                validation.Message,
                validation.Title,
                validation.Code,
                validation.Errors),
            BusinessRuleException business => CreateProblemDetails(
                httpContext,
                StatusCodes.Status400BadRequest,
                business.Message,
                business.Title,
                business.Code),
            ArgumentException argEx => CreateProblemDetailsFromArgumentException(
                httpContext,
                argEx),
            _ => CreateProblemDetails(
                httpContext,
                StatusCodes.Status500InternalServerError,
                _env.IsDevelopment() ? exception.Message : "An unexpected error occurred.",
                "Server Error",
                null)
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }

    private static ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int status,
        string detail,
        string title,
        string? code = null,
        IDictionary<string, string[]>? errors = null)
    {
        var problem = new ProblemDetails
        {
            Status = status,
            Type = $"https://httpstatuses.com/{status}",
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        if (code != null)
        {
            problem.Extensions["code"] = code;
        }

        if (errors != null && errors.Any())
        {
            problem.Extensions["errors"] = errors;
        }

        return problem;
    }

    private ProblemDetails CreateProblemDetailsFromArgumentException(
        HttpContext httpContext,
        ArgumentException exception)
    {
        string detail;
        if (_env.IsDevelopment())
        {
            detail = exception.Message; // Include parameter name in dev
        }
        else
        {
            detail = "The request contained invalid data."; // Generic in prod
        }

        return CreateProblemDetails(
            httpContext,
            StatusCodes.Status400BadRequest,
            detail,
            "Validation Error",
            "VALIDATION_ERROR");
    }
}