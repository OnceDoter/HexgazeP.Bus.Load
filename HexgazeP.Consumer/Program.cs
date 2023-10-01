using HexgazeP.Common;
using HexgazeP.Consumer;
using HexgazeP.RabbitMQMessageGenerator;
using MassTransit;
using MassTransit.ActivityTracing;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

Host.CreateDefaultBuilder(args)
    .ConfigureServices(static (_, x) =>
    {
        x.AddOpenTelemetry()
            .WithTracing(static x => 
                x.ConfigureResource(static x => 
                        x.AddService("HexgazeP.Consumer"))
                    .AddHttpClientInstrumentation()
                    .AddMassTransitInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddSource("*")
                    .AddOtlpExporter(static x =>
                    {
                        x.Endpoint = new Uri(Environment.GetEnvironmentVariable(EnvVars.TraceEndpoint) ?? "http://localhost:4317/");
                        x.Protocol = OtlpExportProtocol.Grpc;
                    }));
        x.AddHttpClient();
        x.AddMassTransit(static x =>
        {
            x.UsingRabbitMq(static (_, x) =>
            {
                x.PropagateActivityTracingContext();
                x.Host(new Uri(Environment.GetEnvironmentVariable(EnvVars.BrokerHost) ?? "amqp://guest:guest@localhost:5672/"), static x =>
                {
                    x.RequestedConnectionTimeout(TimeSpan.FromSeconds(3));
                });
                
                x.ReceiveEndpoint(nameof(Message), static x =>
                {
                    x.PrefetchCount = int.Parse(Environment.GetEnvironmentVariable(EnvVars.BrokerPrefetch) ?? "50");
                    x.ExchangeType = "direct";
                    x.ConfigureConsumeTopology = false;
                    x.Consumer<Consumer>();
                });
                
                x.ConfigureEndpoints(_);
            });
        });
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
    }).Start().WaitForShutdown();