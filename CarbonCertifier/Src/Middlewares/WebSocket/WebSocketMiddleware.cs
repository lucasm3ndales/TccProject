using CarbonCertifier.Services.Wss;

namespace CarbonCertifier.Middlewares.WebSocketMiddleware;

public class WebSocketMiddleware(RequestDelegate next, WebSocketHostedService webSocketHostedService)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (httpContext.Request.Path == "/ws" && httpContext.WebSockets.IsWebSocketRequest)
        {
            await webSocketHostedService.HandleWebSocketConnectionAsync(httpContext);
        }
        else
        {
            await next(httpContext);
        }
    }
}