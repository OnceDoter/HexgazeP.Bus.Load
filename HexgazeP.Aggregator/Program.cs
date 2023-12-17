using System.Diagnostics;
using HexgazeP.Aggregator;
using HexgazeP.Common;
using HexgazeP.RabbitMQMessageGenerator;
using OpenTelemetry.Trace;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.AddRedis("redis");
builder.AddServiceDefaults();
var services = builder.Services;
services.AddHttpClient();
services.AddHostedService<Sender>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapPost("/", async (HttpContext ctx, IConnectionMultiplexer connectionMultiplexer, ILogger<Program> logger) =>
{
    try
    {
        using var reader = new StreamReader(ctx.Request.Body);
        var requestBody = await reader.ReadToEndAsync();
        var client = connectionMultiplexer.GetDatabase();
        await client.ListRightPushAsync(
            Environment.GetEnvironmentVariable(EnvVars.RabbitQueueName) ?? typeof(Message).FullName, 
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