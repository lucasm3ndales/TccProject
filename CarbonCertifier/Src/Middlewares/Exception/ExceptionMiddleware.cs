using System.Net;
using System.Text.Json;
using CarbonCertifier.Middlewares.ExceptionMiddleware.Dtos;

namespace CarbonCertifier.Middlewares.ExceptionMiddleware;

public class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext, Exception ex)
    {
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        
        var response = BuildHttpResponse(ex);

        var json = JsonSerializer.Serialize(response);

        await httpContext.Response.WriteAsync(json);
    }

    private ExceptionResponseDto BuildHttpResponse(Exception ex)
    {
        return ex switch
        {
            NullReferenceException => new ExceptionResponseDto(HttpStatusCode.NotFound, "Not Found Resource."),
            ArgumentException => new ExceptionResponseDto(HttpStatusCode.BadRequest, "Arguments of request are invalid."),
            KeyNotFoundException => new ExceptionResponseDto(HttpStatusCode.NotFound, "Key Not Found."),
            _ => new ExceptionResponseDto(HttpStatusCode.InternalServerError, "Internal server error.")
        };
    }

}