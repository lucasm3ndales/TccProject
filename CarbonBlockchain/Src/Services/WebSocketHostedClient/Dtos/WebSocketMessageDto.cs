using System.Text.Json.Serialization;

namespace CarbonBlockchain.Services.WebSocketHosted.Dtos;

public class WebSocketMessageDto(int StatusCode, long Timestamp, object? Message)
{
    public int StatusCode { get; set; } = StatusCode;
    public long Timestamp { get; set; } = Timestamp;
    public object? Message { get; set; } = Message;
}