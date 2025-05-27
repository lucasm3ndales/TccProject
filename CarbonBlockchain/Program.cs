using System.Net;
using CarbonBlockchain.Data;
using CarbonBlockchain.Middlewares.Exception;
using CarbonBlockchain.Services;
using CarbonBlockchain.Services.Account;
using CarbonBlockchain.Services.BesuClient;
using CarbonBlockchain.Services.CarbonCreditHandler;
using CarbonBlockchain.Services.WebSocketHosted;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<CarbonBlockchainDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbStringConnection")));

builder.Services.AddScoped<ICarbonCreditHandlerService, CarbonCreditHandlerService>();
builder.Services.AddScoped<IBesuClientService, BesuClientService>();
builder.Services.AddScoped<IAccountService, AccountService>();


builder.Services.AddSingleton<IWebSocketHostedClientService, WebSocketHostedClientService>();
builder.Services.AddSingleton<IBesuEventHostedClientService, BesuEventHostedClientService>();

builder.Services.AddHostedService<BesuEventHostedClientService>();
builder.Services.AddHostedService<WebSocketHostedClientService>();

builder.WebHost.ConfigureKestrel(options => options.Listen(IPAddress.Any, 5307));

builder.Services.AddControllers();

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.MapOpenApi();
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();
app.UseWebSockets();
app.Run();