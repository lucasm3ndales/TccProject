using System.Text.Json.Serialization;

namespace CarbonBlockchain.Services.WebSocketHosted.Dtos;

public class WebSocketMessageDto(int StatusCode, long Timestamp, object Message)
{
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; } = StatusCode;
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; } = Timestamp;
    [JsonPropertyName("message")]
    public object? Message { get; set; } = Message;
}