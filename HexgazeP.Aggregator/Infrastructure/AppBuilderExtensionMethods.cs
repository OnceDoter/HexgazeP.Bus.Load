namespace HexgazeP.Aggregator.Infrastructure;

public static class AppBuilderExtensionMethods
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder, Action<IServiceCollection> configure)
    {
        configure(builder.Services);
        return builder;
    }
}