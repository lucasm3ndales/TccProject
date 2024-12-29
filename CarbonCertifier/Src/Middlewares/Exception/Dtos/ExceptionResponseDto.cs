using System.Net;

namespace CarbonCertifier.Middlewares.ExceptionMiddleware.Dtos;

public record ExceptionResponseDto(HttpStatusCode StatusCode, string Message)
{
    
}