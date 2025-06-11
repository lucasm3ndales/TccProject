
namespace CarbonBlockchain.Services.WebSocketHostedClient.Dtos;

public class WebSocketMessageDto
{
    public int? StatusCode { get; set; }
    public long? Timestamp { get; set; }
    public string? Message { get; set; }

    public WebSocketMessageDto() { }

    public WebSocketMessageDto(int? statusCode, long? timestamp, string? message)
    {
        StatusCode = statusCode;
        Timestamp = timestamp;
        Message = message;
    }
}
