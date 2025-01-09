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
            Console.WriteLine($"Exception catch: {ex.Message}");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
    {
        var response = BuildHttpResponse(ex);
        
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = response.StatusCode;
        
        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private ExceptionResponseDto BuildHttpResponse(Exception ex)
    {
        return ex switch
        {
            ArgumentOutOfRangeException =>  new ExceptionResponseDto((int)HttpStatusCode.BadRequest, GetMessage(ex, "Arguments of request are out of range of valid values.")),
            ArgumentNullException => new ExceptionResponseDto((int)HttpStatusCode.BadRequest, GetMessage(ex, "Some arguments of request are null.")),
            ArgumentException => new ExceptionResponseDto((int)HttpStatusCode.BadRequest, GetMessage(ex, "Some arguments of request are invalid.")),
            NullReferenceException => new ExceptionResponseDto((int)HttpStatusCode.NotFound, GetMessage(ex, "Not Found Resource.")),
            _ => new ExceptionResponseDto((int)HttpStatusCode.InternalServerError, GetMessage(ex, "Internal server error."))
        };
    }

    private string GetMessage(Exception ex, string message)
    {
        return string.IsNullOrEmpty(ex.Message) ? message : ex.Message;
    }

}