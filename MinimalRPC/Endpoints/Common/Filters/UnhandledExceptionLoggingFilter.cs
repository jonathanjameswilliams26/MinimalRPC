namespace MinimalRPC.Endpoints.Common.Filters;

public class UnhandledExceptionLoggingFilter<TRequest, TResponse> : EndpointFilter<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<UnhandledExceptionLoggingFilter<TRequest, TResponse>> logger;

    public UnhandledExceptionLoggingFilter(ILogger<UnhandledExceptionLoggingFilter<TRequest, TResponse>> logger)
    {
        this.logger = logger;
    }

    protected override async Task<object?> Handle(TRequest request, EndpointFilterInvocationContext context, EndpointFilterDelegate next, CancellationToken cancellationToken)
    {
        try
        {
            return await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError
            (
                exception: ex, 
                message: "{Request}: An unexpected error occurred. Request Data: {@RequestData}", 
                request.GetType().Name, 
                SensitiveRequests.CanLogRequestBody(request) ? request : null
            );
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
