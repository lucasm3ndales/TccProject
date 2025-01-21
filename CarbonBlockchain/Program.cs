using System.Net;
using CarbonBlockchain.Data;
using CarbonBlockchain.Middlewares.Exception;
using CarbonBlockchain.Services.WebSocketHosted;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<CarbonBlockchainDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbStringConnection")));



builder.Services.AddSingleton<IWebSocketService, WebSocketService>();

builder.Services.AddHostedService<WebSocketService>();

builder.WebHost.ConfigureKestrel(options => options.Listen(IPAddress.Any, 5309));

builder.Services.AddControllers();

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.MapOpenApi();
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();
app.UseWebSockets();
app.Run();