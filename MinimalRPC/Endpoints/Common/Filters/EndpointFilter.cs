namespace MinimalRPC.Endpoints.Common.Filters;

public abstract class EndpointFilter<TRequest, TResponse> : IEndpointFilter where TRequest : notnull
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.GetArgument<TRequest>(0);
        var cancellationToken = context.GetArgument<CancellationToken>(2);
        return await Handle(request, context, next, cancellationToken);
    }

    protected abstract Task<object?> Handle(TRequest request, EndpointFilterInvocationContext context, EndpointFilterDelegate next, CancellationToken cancellationToken);
}