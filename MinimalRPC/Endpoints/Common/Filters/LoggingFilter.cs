using System.Diagnostics;

namespace MinimalRPC.Endpoints.Common.Filters;

public class LoggingFilter<TRequest, TResponse> : EndpointFilter<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingFilter<TRequest, TResponse>> logger;

    public LoggingFilter(ILogger<LoggingFilter<TRequest, TResponse>> logger)
    {
        this.logger = logger;
    }

    protected override async Task<object?> Handle(TRequest request, EndpointFilterInvocationContext context, EndpointFilterDelegate next, CancellationToken cancellationToken)
    {
        var requestType = request.GetType().Name;
        var stopwatch = Stopwatch.StartNew();
        var canLogRequestBody = logger.IsEnabled(LogLevel.Debug) && SensitiveRequests.CanLogRequestBody(request);

        logger.LogInformation("{Request}: received", requestType);
        if (canLogRequestBody)
        {
            logger.LogDebug("{Request} Data: {@RequestData}", requestType, request);
        }

        var response = await next(context);
        var statusCodeResult = response as IStatusCodeHttpResult;

        stopwatch.Stop();
        logger.LogInformation("{Request}: Finished in {MS}ms. Status Code: {StatusCode}", request.GetType().Name, stopwatch.ElapsedMilliseconds, statusCodeResult?.StatusCode);
        return response;
    }
}
