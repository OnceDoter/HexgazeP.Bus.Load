using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using HexgazeP.Common;
using HexgazeP.RabbitMQMessageGenerator;
using MassTransit;
using Newtonsoft.Json;
using OpenTelemetry.Trace;

namespace HexgazeP.Consumer;

public class Consumer : IConsumer<Message>
{
    private readonly HttpClient _client;
    
    public Consumer()
    {
        _client = new HttpClient();
    }
    
    public async Task Consume(ConsumeContext<Message> context)
    {
        try
        {
            var stringPayload = JsonConvert.SerializeObject(context.Message);
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            await _client.PostAsync(Environment.GetEnvironmentVariable(EnvVars.PostEndpoint) ?? "http://localhost:5002/", httpContent);
        }
        catch (Exception e)
        {
            Activity.Current?.RecordException(e);
            throw;
        }
    }
}
