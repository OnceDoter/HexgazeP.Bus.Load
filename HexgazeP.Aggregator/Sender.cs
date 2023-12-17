using System.Text;
using HexgazeP.Common;
using HexgazeP.RabbitMQMessageGenerator;
using StackExchange.Redis;

namespace HexgazeP.Aggregator;

public sealed class Sender : BackgroundService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<Sender> _logger;

    public Sender(IConnectionMultiplexer connectionMultiplexer, IHttpClientFactory httpClientFactory, ILogger<Sender> logger)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                var _redisClientAsync = _connectionMultiplexer.GetDatabase();
                var batch = await _redisClientAsync.ListRangeAsync(Environment.GetEnvironmentVariable(EnvVars.RabbitQueueName) ?? typeof(Message).FullName, 0, 5000);
                if (batch.Length == 0)
                {
                    await Task.Delay(5000, token);
                    continue;
                }
                var httpContent = new StringContent($"[{string.Join(", ", batch.ToStringArray())}]", Encoding.UTF8, "application/json");
                await _httpClientFactory.CreateClient().PostAsync("http://localhost:5202/saveBatch", httpContent, token);
                await _redisClientAsync.ListTrimAsync(Environment.GetEnvironmentVariable(EnvVars.RabbitQueueName) ?? typeof(Message).FullName, batch.Length, -1);
                
                _logger.LogInformation("Batch sent!");

                if (batch.Length < 5000)
                {
                    await Task.Delay(1000, token);
                }
            }
            catch (Exception e)
            {
                await Task.Delay(5000, token);
                _logger.LogError("Error: {Message}", e.Message);
            }
        }
    }
}

public static class Ext
{
    public static string[] ToStringArray(this RedisValue[] values)
    {
        if (values == null) return null;
        if (values.Length == 0) return Array.Empty<string>();
        return Array.ConvertAll(values, x => (string)x);
    }
}