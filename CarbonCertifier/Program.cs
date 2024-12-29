using CarbonCertifier.Data;
using CarbonCertifier.Middlewares.ExceptionMiddleware;
using CarbonCertifier.Middlewares.WebSocketMiddleware.extensions;
using CarbonCertifier.Services.CarbonCredit;
using CarbonCertifier.Services.CarbonProject;
using CarbonCertifier.Services.Wss;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<ICarbonProjectService, CarbonProjectService>();

builder.Services.AddScoped<ICarbonCreditService, CarbonCreditService>();

builder.Services.AddHostedService<WebSocketHostedService>();

builder.Services.AddDbContext<CarbonCertifierDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbStringConnection")));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();

var app = builder.Build();

app.MapOpenApi();
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();
app.UseHttpsRedirection();
app.UseWebSocketMiddleware();
app.Run();
