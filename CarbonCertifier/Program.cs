using System.Net;
using System.Text.Json.Serialization;
using CarbonCertifier.Data;
using CarbonCertifier.Middlewares.Exception;
using CarbonCertifier.Services.CarbonCredit;
using CarbonCertifier.Services.CarbonProject;
using CarbonCertifier.Services.WebSocketHostedServer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<CarbonCertifierDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbStringConnection")));

builder.Services.AddScoped<ICarbonProjectService, CarbonProjectService>();
builder.Services.AddScoped<ICarbonCreditService, CarbonCreditService>();

builder.Services.AddSingleton<IWebSocketHostedServerService, WebSocketHostedServerService>();

builder.Services.AddHostedService<WebSocketHostedServerService>();

builder.WebHost.ConfigureKestrel(options => options.Listen(IPAddress.Any, 5207));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.MapOpenApi();
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();
app.UseWebSockets();
app.Run();

