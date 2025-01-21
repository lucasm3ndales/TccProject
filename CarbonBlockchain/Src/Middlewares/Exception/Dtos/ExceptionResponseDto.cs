
namespace CarbonBlockchain.Middlewares.Exception.Dtos;

public class ExceptionResponseDto(int StatusCode, string Message)
{
    public int StatusCode { get; set; } = StatusCode;
    public string Message { get; set; } = Message;
}