using System.Diagnostics;
using HexgazeP.Aggregator;
using HexgazeP.RabbitMQMessageGenerator;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using OpenTelemetry.Trace;
using ServiceStack.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
var services = builder.Services;
services.AddHttpClient();
services.AddHostedService<Sender>();
var redisManager = new RedisManagerPool(Environment.GetEnvironmentVariable(EnvVars.RedisEndpoint) ?? "localhost:6379");
var redisClient = await redisManager.GetClientAsync();
services.AddSingleton(redisClient);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Listen(System.Net.IPAddress.Any, 5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
});


var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/", async (HttpContext ctx, IRedisClientAsync redisClientAsync, ILogger<Program> logger) =>
{
    try
    {
        using var reader = new StreamReader(ctx.Request.Body);
        var requestBody = await reader.ReadToEndAsync();
        await redisClientAsync.AddItemToListAsync(
            Environment.GetEnvironmentVariable(EnvVars.RabbitQueueName), 
            requestBody);
            
        logger.LogInformation("Message received successfully");
    }
    catch (Exception e)
    {
        Activity.Current?.RecordException(e);
        logger.LogError("Message error {Error}", e.Message);
    }
});

app.Run();