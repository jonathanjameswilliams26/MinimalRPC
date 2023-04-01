using MinimalRPC.Endpoints.Common;

namespace MinimalRPC.Endpoints.DependencyInjection;

public static class Configure
{
    public static void UseEndpoints(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var endpoints = scope.ServiceProvider.GetServices<IEndpoint>().ToList();
        endpoints.ForEach(x => 
        {
            var configuration = x.Map(app);
            x.Configure(configuration);
        });
    }
}