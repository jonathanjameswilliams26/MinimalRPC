using MinimalRPC.Endpoints.Common;

namespace MinimalRPC.Endpoints.DependencyInjection;

public static class ConfigureServices
{
    public static void AddEndpoints(this IServiceCollection services)
    {
        services.Scan(scan =>
            scan.FromAssemblyOf<IEndpoint>()
            .AddClasses(classes => classes.AssignableTo<IEndpoint>())
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );
    }
}
