using System.Text.Json;

namespace CarbonCertifier.Services.WebSocketHosted.Dtos;
public class WebSocketMessageDto(int statusCode, long timestamp, object? message)
{
    public int? StatusCode { get; set; } = statusCode;
    public long? Timestamp { get; set; } = timestamp;
    public object? Message { get; set; } = message;
}
