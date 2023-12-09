using System.Diagnostics;
using HexgazeP.Common;
using HexgazeP.RabbitMQMessageGenerator;
using Microsoft.Extensions.Caching.Memory;
using OpenTelemetry.Trace;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
var services = builder.Services;
services.AddTransient<IDatabase>(_ => ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable(EnvVars.RedisEndpoint) ?? "localhost:6379").GetDatabase());
services.AddSingleton<Sequence>();
services.AddSingleton<IMemoryCache, MemoryCache>();
var app = builder.Build();

app.MapDefaultEndpoints();

app.MapPost("/{queue}", async (HttpContext ctx, string queue, IDatabase redisDatabase) =>
{
    await using var memoryStream = new MemoryStream();
    await ctx.Request.BodyReader.CopyToAsync(memoryStream);
    await redisDatabase.ListLeftPushAsync(queue, memoryStream.ToArray());
});

app.MapGet("/{queue}", async (string queue, IDatabase db, Sequence seq, IMemoryCache mem) =>
{
    using var entry = mem.CreateEntry(Activity.Current?.Id ?? Guid.NewGuid().ToString());
    entry.Value = seq.NextValue();
    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(int.Parse(Environment.GetEnvironmentVariable(EnvVars.AckTimeoutSeconds) ?? "5000"));
    return new
    {
        TransactionId = entry.Key,
        Batch = (await db.ListRangeAsync(queue, start: (long)entry.Value - int.Parse(Environment.GetEnvironmentVariable(EnvVars.BatchSize) ?? "5000"), stop: (long)entry.Value))
            .Select(x => (byte[])x!).ToArray()
    } ;
});

app.MapPut("/{queue}/{transactionId}", async (HttpContext ctx, string queue, string transactionId, IDatabase redisDatabase, IMemoryCache mem) =>
{
    if (mem.TryGetValue(transactionId, out long value))
    {
        await redisDatabase.ListTrimAsync(queue, start: (long)value - int.Parse(Environment.GetEnvironmentVariable(EnvVars.BatchSize) ?? "5000"), stop: value);
        mem.Remove(transactionId);
    }
    else
    {
        ctx.Response.StatusCode = 404;
    }   
});

app.Use(next => async ctx =>
{
    try
    {
        await next(ctx);
        ctx.Response.StatusCode = 200;
    }
    catch (Exception e)
    {
        var activity = Activity.Current;
        switch (activity)
        {
            case null:
                Console.WriteLine(e);
                break;
            default:
                activity?.RecordException(e);
                break;
        }

        ctx.Response.StatusCode = 500;
    }
});

app.Run();