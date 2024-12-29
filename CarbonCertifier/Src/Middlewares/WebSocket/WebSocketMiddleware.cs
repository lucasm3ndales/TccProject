using CarbonCertifier.Services.Wss;

namespace CarbonCertifier.Middlewares.WebSocketMiddleware;

public class WebSocketMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext, WebSocketHostedService webSocketService)
    {
        if (httpContext.Request.Path == "/ws" && httpContext.WebSockets.IsWebSocketRequest)
        {
            await webSocketService.HandleWebSocketConnectionAsync(httpContext);
        }
        else
        {
            await next(httpContext);
        }
    }
}