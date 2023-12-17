using HexgazeP.Common;
using HexgazeP.Consumer;
using HexgazeP.RabbitMQMessageGenerator;
using MassTransit;
using MassTransit.ActivityTracing;
using RabbitMQ.Client;


var builder = Host.CreateApplicationBuilder(args);

builder.AddRabbitMQ("rabbitmq");
builder.AddServiceDefaults();
var services = builder.Services;

services.AddHttpClient();
services.AddHttpLogging(static _ => { });
services.AddMassTransit(static x =>
{
    x.UsingRabbitMq(static (context, x) =>
    {
        var connectionFactory = (ConnectionFactory)context.GetRequiredService<IConnectionFactory>(); // todo: в общем aspire и mass transit не колабятся так просто
        x.PropagateActivityTracingContext();
        x.Host(new Uri(connectionFactory.Endpoint.ToString()), static x =>
        {
            x.RequestedConnectionTimeout(TimeSpan.FromMinutes(10));
        });
        
        x.ReceiveEndpoint(nameof(Message), static x =>
        {
            x.BindQueue = true;
            x.PrefetchCount = int.Parse(Environment.GetEnvironmentVariable(EnvVars.BrokerPrefetch) ?? "5");
            x.ExchangeType = ExchangeType.Direct;
            x.ConfigureConsumeTopology = false;
            x.Consumer<Consumer>();
            
            x.Bind(nameof(Message), static x =>
            {
                x.ExchangeType = ExchangeType.Direct;
            });
            
        });

        x.ConfigureEndpoints(context);
    });
});

builder.Build().Run();