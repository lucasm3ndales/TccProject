namespace CarbonCertifier.Services.Wss.Dtos;

public class WebSocketDataDto(int StatusCode, long Timestamp, string Message)
{
    public int StatusCode { get; set; } = StatusCode;
    public long Timestamp { get; set; } = Timestamp;
    public string Message { get; set; } = Message;
}