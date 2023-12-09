using HexgazeP.Common;
using HexgazeP.RabbitMQMessageGenerator;
using MassTransit;
using MassTransit.ActivityTracing;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
var services = builder.Services;

services.AddMassTransit(static x =>
{
    x.UsingRabbitMq(static (_, x) =>
    {
        x.PropagateActivityTracingContext();
        x.Host(new Uri(Environment.GetEnvironmentVariable(EnvVars.BrokerHost) ?? "amqp://guest:guest@localhost:5672/"), static x =>
        {
            x.RequestedConnectionTimeout(TimeSpan.FromSeconds(3));

            x.ConfigureBatchPublish(static x =>
            {
                x.Enabled = true;
                x.Timeout = TimeSpan.FromMilliseconds(2);
            });
        });

        x.Message<Message>(static x =>
        {
            x.SetEntityName(nameof(Message));
        });

        x.Send<Message>(static x => x.UseRoutingKeyFormatter(context => context.Message.GetType().Name));
        x.Publish<Message>(static x => { x.ExchangeType = ExchangeType.Direct; });
    });
});

services.AddHostedService<Worker>();

builder.Build().Run();