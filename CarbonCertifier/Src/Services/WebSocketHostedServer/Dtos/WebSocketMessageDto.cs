using System.Text.Json;

namespace CarbonCertifier.Services.WebSocketHosted.Dtos;
public class WebSocketMessageDto
{
    public int? StatusCode { get; set; }
    public long? Timestamp { get; set; }
    public object? Message { get; set; }

    public WebSocketMessageDto() { }

    public WebSocketMessageDto(int? statusCode, long? timestamp, object? message)
    {
        StatusCode = statusCode;
        Timestamp = timestamp;
        Message = message;
    }
}

