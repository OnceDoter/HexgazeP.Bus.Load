namespace HexgazeP.RabbitMQMessageGenerator;

public static class EnvVars
{
    public const string BrokerHost = "BROKER_HOST";
    public const string TraceEndpoint = "TRACE_ENDPOINT";
    public const string PostEndpoint = "POST_ENDPOINT";
    public const string BatchSize = "BATCH_SIZE";
    public const string BrokerPrefetch = "BROKER_PREFETCH";
    public const string RabbitQueueName = "RABBIT_QUEUE_NAME";
    public const string RedisEndpoint = "REDIS_ENDPOINT";
    public static string AckTimeoutSeconds { get; set; }
}