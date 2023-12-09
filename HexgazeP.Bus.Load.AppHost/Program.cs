var builder = DistributedApplication.CreateBuilder(args);

builder.AddRabbitMQContainer("rabbitmq");

builder.AddRedisContainer("redis");

builder.AddProject<Projects.HexgazeP_RabbitMQMessageGenerator>("hexgazep.rabbitmqmessagegenerator");

builder.AddProject<Projects.HexgazeP_Consumer>("hexgazep.consumer");

builder.AddProject<Projects.HexgazeP_BatchQueue>("hexgazep.batchqueue");

builder.AddProject<Projects.HexgazeP_API>("hexgazep.api");

builder.AddProject<Projects.HexgazeP_Aggregator>("hexgazep.aggregator");

builder.Build().Run();
