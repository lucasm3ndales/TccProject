using System.Net;
using CarbonCertifier.Data;
using CarbonCertifier.Middlewares.ExceptionMiddleware;
using CarbonCertifier.Services.CarbonCredit;
using CarbonCertifier.Services.CarbonProject;
using CarbonCertifier.Services.Wss;
using Microsoft.AspNetCore.WebSockets;
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

builder.Services.AddSingleton<WebSocketHostedService>();

builder.Services.AddHostedService<WebSocketHostedService>();

builder.WebHost.ConfigureKestrel(options => options.Listen(IPAddress.Any, 5207));

builder.Services.AddControllers();

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.MapOpenApi();
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();
app.UseWebSockets();
app.Run();
