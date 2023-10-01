using HexgazeP.API;
using HexgazeP.Common;
using HexgazeP.RabbitMQMessageGenerator;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using DbContext = HexgazeP.API.DbContext;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenTelemetry()
    .WithTracing(static x => 
        x.ConfigureResource(static x => 
                x.AddService("HexgazeP.API"))
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddSource("*")
            .AddOtlpExporter(static x =>
            {
                x.Endpoint = new Uri(Environment.GetEnvironmentVariable(EnvVars.TraceEndpoint) ?? "http://localhost:4317/");
                x.Protocol = OtlpExportProtocol.Grpc;
            }));
builder.Services.AddDbContext<DbContext>(static x => x.UseSqlite("Data Source=hexgazep.db"));

var app = builder.Build();

app.MapPost("/saveBatch", async (Message[] messages, DbContext dbContext, ILogger<Message> logger) =>
{
    logger.LogInformation("Received {Length} messages", messages.Length);
    
    // Сохраните пакет сообщений в базу данных
    dbContext.Messages.AddRange(messages.Select(x => new DbContext.Message
    {
        GuidProperty = x.GuidProperty,
        IntProperty = x.IntProperty,
        StringProperty = x.StringProperty,
        DoubleProperty = x.DoubleProperty,
        BoolProperty = x.BoolProperty,
        DateTimeProperty = x.DateTimeProperty,
        DecimalProperty = x.DecimalProperty,
        CharProperty = x.CharProperty,
        LongProperty = x.LongProperty,
        FloatProperty = x.FloatProperty,
        ByteProperty = x.ByteProperty,
        ShortProperty = x.ShortProperty,
        UShortProperty = x.UShortProperty,
        UIntProperty = x.UIntProperty,
        ULongProperty = x.ULongProperty,
        ByteArrayProperty = x.ByteArrayProperty,
        TimeSpanProperty = x.TimeSpanProperty,
        UriProperty = x.UriProperty,
        VersionProperty = x.VersionProperty,
        ObjectProperty = x.ObjectProperty,
        NestedClassProperty = x.NestedClassProperty.NestedStringProperty
    }));
    
    await dbContext.SaveChangesAsync();
});

app.MapGet("/", async (DbContext dbContext) => await dbContext.Messages.ToArrayAsync());


app.Run();