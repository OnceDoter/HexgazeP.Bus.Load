using HexgazeP.Common;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using DbContext = HexgazeP.API.DbContext;

var builder = WebApplication.CreateBuilder(args);
builder.AddNpgsqlDbContext<DbContext>("postgres");
builder.AddServiceDefaults();
var services = builder.Services;
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var app = builder.Build();

app.MapDefaultEndpoints();

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
        TimeSpanProperty = x.TimeSpanProperty,
        NestedClassProperty = x.NestedClassProperty.NestedStringProperty
    }));
    
    await dbContext.SaveChangesAsync();
});

app.MapGet("/", async (DbContext dbContext) => await dbContext.Messages.ToArrayAsync());


app.Run();