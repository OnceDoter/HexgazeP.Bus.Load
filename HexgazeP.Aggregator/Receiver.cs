using System.Text.Json;
using FastEndpoints;
using HexgazeP.Common;
using HexgazeP.RabbitMQMessageGenerator;
using Microsoft.AspNetCore.Authorization;
using ServiceStack.Redis;

namespace HexgazeP.Aggregator;

[HttpPost("/")]
[AllowAnonymous]
public sealed class Receiver : Endpoint<Message>
{
    private readonly IRedisClientAsync _redisClientAsync;
    private readonly ILogger<Receiver> _logger;

    public Receiver(IRedisClientAsync redisClientAsync, ILogger<Receiver> logger)
    {
        _redisClientAsync = redisClientAsync;
        _logger = logger;
    }

    public override async Task HandleAsync(Message o, CancellationToken token)
    {
        try
        {
            await _redisClientAsync.AddItemToListAsync(
                Environment.GetEnvironmentVariable(EnvVars.RabbitQueueName) ?? o.GetType().FullName, 
                JsonSerializer.Serialize(o),
                token);
            
            _logger.LogInformation("Message received {Message}", o.GuidProperty);
        }
        catch (Exception e)
        {
            _logger.LogError("Message received {Error}", e.Message);
            await SendAsync(e, cancellation: token);
        }
    }
}