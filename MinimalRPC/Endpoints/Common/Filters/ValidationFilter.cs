namespace MinimalRPC.Endpoints.Common.Filters;

public class ValidationFilter<TRequest, TResponse> : EndpointFilter<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> validators;
    private readonly ILogger<ValidationFilter<TRequest, TResponse>> logger;

    public ValidationFilter(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationFilter<TRequest, TResponse>> logger)
    {
        this.validators = validators;
        this.logger = logger;
    }

    protected override async Task<object?> Handle(TRequest request, EndpointFilterInvocationContext context, EndpointFilterDelegate next, CancellationToken cancellationToken)
    {
        var requestType = request.GetType().Name;
        if (validators.Any() == false)
        {
            logger.LogDebug("{Request}: No request validators configured.", requestType);
            return await next(context);
        }

        logger.LogDebug("{Request}: Running request validations.", requestType);
        var validationContext = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(validationContext, cancellationToken)));
        var failure = validationResults.SelectMany(r => r.Errors).FirstOrDefault(f => f != null);

        if (failure != null)
        {
            var badRequest = new BadRequest(failure);
            logger.LogDebug("{Request}: Validation failed. Error: {Error}.", requestType, badRequest);
            return Results.BadRequest(badRequest);
        }

        logger.LogDebug("{Request}: Request validation successful.", requestType);
        return await next(context);
    }
}
