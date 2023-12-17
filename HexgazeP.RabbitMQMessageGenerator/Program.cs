using HexgazeP.Common;
using HexgazeP.RabbitMQMessageGenerator;
using MassTransit;
using MassTransit.ActivityTracing;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);

builder.AddRabbitMQ("rabbitmq");
builder.AddServiceDefaults();
var services = builder.Services;

services.AddMassTransit(static x =>
{
    x.UsingRabbitMq(static (context, x) =>
    {
        var connectionFactory = (ConnectionFactory)context.GetRequiredService<IConnectionFactory>(); // todo: в общем aspire и mass transit не колабятся так просто
        x.PropagateActivityTracingContext();
        x.Host(new Uri(connectionFactory.Endpoint.ToString()), static x =>
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

        x.Send<Message>(static x =>
        {
            x.UseRoutingKeyFormatter(static _ => string.Empty);
        });
        x.Publish<Message>(static x => { x.ExchangeType = ExchangeType.Direct; });
    });
});

services.AddHostedService<Worker>();

builder.Build().Run();