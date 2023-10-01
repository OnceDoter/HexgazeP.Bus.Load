using HexgazeP.Common;
using HexgazeP.RabbitMQMessageGenerator;
using MassTransit;
using MassTransit.ActivityTracing;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using RabbitMQ.Client;

Host.CreateDefaultBuilder(args)
    .ConfigureServices(static (_, x) =>
    {
        x.AddOpenTelemetry()
            .WithTracing(static x => 
                x.ConfigureResource(static x => 
                        x.AddService("HexgazeP.RabbitMQMessageGenerator"))
                    .AddHttpClientInstrumentation()
                    .AddMassTransitInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddSource("*")
                    .AddOtlpExporter(static x =>
                    {
                        x.Endpoint = new Uri(Environment.GetEnvironmentVariable(EnvVars.TraceEndpoint) ?? "http://localhost:4317/");
                        x.Protocol = OtlpExportProtocol.Grpc;
                    }));
        x.AddMassTransit(static x =>
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
                
                // Определите класс сообщения для публикации
                x.Message<Message>(static x =>
                {
                    x.SetEntityName(nameof(Message));
                });

                // Определите обменник с именем, равным имени сообщения
                x.Send<Message>(static x => x.UseRoutingKeyFormatter(context => context.Message.GetType().Name));
                x.Publish<Message>(static x => { x.ExchangeType = ExchangeType.Direct; });
            });
        });

        x.AddHostedService<Worker>();
    }).ConfigureLogging(static (_, x) =>
    {
        x.AddOpenTelemetry(static x =>
        {
            x.AddOtlpExporter(static x =>
            {
                x.Endpoint = new Uri(Environment.GetEnvironmentVariable(EnvVars.TraceEndpoint) ?? "http://localhost:4317/");
                x.Protocol = OtlpExportProtocol.Grpc;
            });
        });
    }).Start().Run();