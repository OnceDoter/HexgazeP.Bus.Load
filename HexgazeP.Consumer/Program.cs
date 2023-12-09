using HexgazeP.Common;
using HexgazeP.Consumer;
using HexgazeP.RabbitMQMessageGenerator;
using MassTransit;
using MassTransit.ActivityTracing;


var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
var services = builder.Services;

services.AddHttpClient();
services.AddMassTransit(static x =>
{
    x.UsingRabbitMq(static (_, x) =>
    {
        x.PropagateActivityTracingContext();
        x.Host(new Uri(Environment.GetEnvironmentVariable(EnvVars.BrokerHost) ?? "amqp://guest:guest@localhost:5672/"), static x =>
        {
            x.RequestedConnectionTimeout(TimeSpan.FromMinutes(10));
        });

        x.ReceiveEndpoint(nameof(Message), static x =>
        {
            x.PrefetchCount = int.Parse(Environment.GetEnvironmentVariable(EnvVars.BrokerPrefetch) ?? "5");
            x.ExchangeType = "direct";
            x.ConfigureConsumeTopology = false;
            x.Consumer<Consumer>();
        });

        x.ConfigureEndpoints(_);
    });
});

builder.Build().Run();