using HexgazeP.Common;
using HexgazeP.RabbitMQMessageGenerator;
using ServiceStack.Redis;
using ServiceStack.Text;

namespace HexgazeP.Aggregator;

public sealed class Sender : BackgroundService
{
    private readonly IRedisClientAsync _redisClientAsync;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<Sender> _logger;

    public Sender(IRedisClientAsync redisClientAsync, IHttpClientFactory httpClientFactory, ILogger<Sender> logger)
    {
        _redisClientAsync = redisClientAsync;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                var batch = await _redisClientAsync.GetRangeFromListAsync(Environment.GetEnvironmentVariable(EnvVars.RabbitQueueName) ?? typeof(Message).FullName, 0, 5000, token);
                if (!batch.Any())
                {
                    await Task.Delay(5000, token);
                    continue;
                }

                var deserialize = batch.Select(JsonSerializer.DeserializeFromString<Message[]>)
                    .SelectMany(x => x).ToList();
                await _httpClientFactory.CreateClient().PostAsync(Environment.GetEnvironmentVariable(EnvVars.PostEndpoint) ?? "http://localhost:5202/saveBatch", JsonContent.Create(deserialize), token);
                await _redisClientAsync.TrimListAsync(Environment.GetEnvironmentVariable(EnvVars.RabbitQueueName) ?? typeof(Message).FullName, batch.Count, -1, token);
                
                _logger.LogInformation("Batch sent!");

                if (batch.Count < 5000)
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