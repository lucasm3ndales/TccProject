namespace CarbonCertifier.Middlewares.WebSocketMiddleware.extensions;

public static class WebSocketMiddlewareExtension
{
    public static IApplicationBuilder UseWebSocketMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<WebSocketMiddleware>();
    }
}