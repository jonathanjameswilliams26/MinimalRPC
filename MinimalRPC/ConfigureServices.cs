using MinimalRPC.Endpoints.DependencyInjection;

namespace MinimalRPC;

public static class ConfigureServicesExtensions
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();
        builder.Services.AddEndpoints();
    }
}
