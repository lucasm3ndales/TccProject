using System.Net;
using System.Text.Json;
using CarbonCertifier.Middlewares.ExceptionMiddleware.Dtos;

namespace CarbonCertifier.Middlewares.ExceptionMiddleware;

public class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
    {
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = (int) GetHttpStatusCode(ex);

        var response = BuildHttpResponse(ex);
        var json = JsonSerializer.Serialize(response);

        await httpContext.Response.WriteAsync(json);
    }

    private HttpStatusCode GetHttpStatusCode(Exception ex)
    {
        return ex switch
        {
            NullReferenceException => HttpStatusCode.NotFound,
            ArgumentException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };
    }

    private ExceptionResponseDto BuildHttpResponse(Exception ex)
    {
        return ex switch
        {
            NullReferenceException => new ExceptionResponseDto(HttpStatusCode.NotFound, GetMessage(ex, "Not Found Resource.")),
            ArgumentException => new ExceptionResponseDto(HttpStatusCode.BadRequest, GetMessage(ex, "Arguments of request are invalid.")),
            _ => new ExceptionResponseDto(HttpStatusCode.InternalServerError, GetMessage(ex, "Internal server error."))
        };
    }

    private string GetMessage(Exception ex, string message)
    {
        return string.IsNullOrEmpty(ex.Message) ? message : ex.Message;
    }

}