using System.Net.Http.Json;
using HexgazeP.Common;
using HexgazeP.RabbitMQMessageGenerator;
using MassTransit;

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
            await _client.PostAsync(Environment.GetEnvironmentVariable(EnvVars.PostEndpoint) ?? "http://localhost:5223/", JsonContent.Create(context.Message));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
