using HexgazeP.Common;
using MassTransit;

namespace HexgazeP.RabbitMQMessageGenerator;

public class Worker : BackgroundService
{
    private readonly IBus _bus;
    private readonly ILogger<Worker> _logger;

    public Worker(IBus bus, ILogger<Worker> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var batchSize = int.Parse(Environment.GetEnvironmentVariable(EnvVars.BatchSize) ?? "1000");
                var batch = Enumerable.Range(0, batchSize).Select(_ => Message.InitializeRandomValues()).ToArray();
                await _bus.PublishBatch(batch, stoppingToken);
                _logger.LogInformation("Published {Length} messages", batch.Length);
                await Task.Delay(1000, stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError("Error while publishing message: {Message}", e.Message);
            }
        }
    }
}