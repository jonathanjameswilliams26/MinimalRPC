using MinimalRPC.Endpoints.Common.Filters;

namespace MinimalRPC.Endpoints.Common;

public interface IEndpoint
{
    RouteHandlerBuilder Map(IEndpointRouteBuilder app);
    void Configure(RouteHandlerBuilder configuration);
}

public interface IEndpoint<TRequest, TResponse> : IEndpoint where TRequest : notnull
{
    Task<OneOf<TResponse, Unathorized, BadRequest>> Handle(TRequest request, CancellationToken cancellationToken);
}

public abstract class Endpoint<TRequest, TResponse> : IEndpoint<TRequest, TResponse> where TRequest : notnull
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapPost(GetType().Name, Execute)
            .WithOpenApi()
            .WithName(GetType().Name)
            .WithTags("All")
            .AddEndpointFilter<LoggingFilter<TRequest, TResponse>>()
            .AddEndpointFilter<UnhandledExceptionLoggingFilter<TRequest, TResponse>>()
            .AddEndpointFilter<ValidationFilter<TRequest, TResponse>>()
            .Produces<TResponse>()
            .Produces<BadRequest>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);
    }
    public abstract void Configure(RouteHandlerBuilder configuration);
    public abstract Task<OneOf<TResponse, Unathorized, BadRequest>> Handle(TRequest request, CancellationToken cancellationToken);

    private static async Task<IResult> Execute(TRequest request, IEndpoint<TRequest, TResponse> endpoint, CancellationToken cancellationToken)
    {
        var result = await endpoint.Handle(request, cancellationToken);
        return result.Match
        (
            success => Results.Ok(success),
            unauthorized => Results.Unauthorized(),
            badRequest => Results.BadRequest(badRequest)
        );
    }
}