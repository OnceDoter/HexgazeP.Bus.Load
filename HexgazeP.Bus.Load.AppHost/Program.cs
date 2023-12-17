var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgresContainer("postgres");

var rabbitmq = builder.AddRabbitMQContainer("rabbitmq");

var redis = builder.AddRedisContainer("redis");

var publisher = builder.AddProject<Projects.HexgazeP_RabbitMQMessageGenerator>("hexgazep.rabbitmqmessagegenerator")
    .WithReference(rabbitmq)
    .WithOtlpExporter();

// var queue = builder.AddProject<Projects.HexgazeP_BatchQueue>("hexgazep.batchqueue").WithReference(redis);

var api = builder.AddProject<Projects.HexgazeP_API>("hexgazep.api")
    .WithReference(postgres)
    .WithOtlpExporter();

var aggregator = builder.AddProject<Projects.HexgazeP_Aggregator>("hexgazep.aggregator")
    .WithReference(redis)
    .WithReference(api)
    .WithOtlpExporter();

var subsriber = builder.AddProject<Projects.HexgazeP_Consumer>("hexgazep.consumer")
    .WithReference(rabbitmq)
    .WithOtlpExporter();

builder.Build().Run();
