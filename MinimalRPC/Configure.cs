using MinimalRPC.Endpoints.DependencyInjection;

namespace MinimalRPC;

public static class ConfigureExtensions
{
    public static void Configure(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints();
    }
}