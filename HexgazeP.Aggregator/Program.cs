using FastEndpoints;
using HexgazeP.Aggregator;
using HexgazeP.Aggregator.Infrastructure;
using HexgazeP.RabbitMQMessageGenerator;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ServiceStack.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .WithTracing(static x => 
        x.ConfigureResource(static x => 
                x.AddService("HexgazeP.Aggregator"))
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddSource("*")
            .AddOtlpExporter(static x =>
            {
                x.Endpoint = new Uri(Environment.GetEnvironmentVariable(EnvVars.TraceEndpoint) ?? "http://localhost:4317/");
                x.Protocol = OtlpExportProtocol.Grpc;
            }));

// реализация антипаттерна;) Просто нужен асинхоронный клиент, а тут нельзя получить его синхранным способом, еще и Configure должен void возвращать
static async Task Configure(IServiceCollection x)
{
    x.AddHttpClient();

    x.AddHostedService<Sender>();
    x.AddFastEndpoints();

    var redisManager = new RedisManagerPool(Environment.GetEnvironmentVariable(EnvVars.RedisEndpoint) ?? "localhost:6379");
    var redisClient = await redisManager.GetClientAsync();
    x.AddSingleton(redisClient);
    
    // Ну дефакто дебаг
    x.AddHealthChecks()
        .AddRedis(Environment.GetEnvironmentVariable(EnvVars.RedisEndpoint) ?? "http://localhost:6379")
        .AddUrlGroup(new Uri(Environment.GetEnvironmentVariable(EnvVars.PostEndpoint) ?? "http://localhost:5202/saveBatch"));
}

var app = builder.ConfigureServices(static x => Configure(x).GetAwaiter().GetResult()).Build();

app.UseFastEndpoints();
app.Run();