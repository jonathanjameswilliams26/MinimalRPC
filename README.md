# MinimalRPC
An opinionated starter project using Minimal APIs with RPC style endpoints following the Request-Endpoint-Response (REPR) pattern.

## Opinionated Design Decisions
1. All endpoints are a `HTTP POST` with a `JSON Request Body`
2. Follows the [REPR Pattern](https://ardalis.com/mvc-controllers-are-dinosaurs-embrace-api-endpoints/) where each endpoint is its own class with its own request/response.
3. Only the following Status Codes are allowed: `200 OK`, `400 Bad Request`, `401 Unathorized`, `500 Server Error`

## Why RPC over REST?
In my opinion and personal experience, RPC api's are:
- Easier to develop since everything is a `HTTP POST`, you only need to use work with a `JSON Request Body` and no longer need route params, query params etc.
- Easier to consume since routes are more descriptive like method names and everything required is in the body.

## Endpoints
### Creating an Endpoint
The following code will register the endpoint `HTTP POST /Example`

Create a request
```csharp
public record ExampleRequest(int Id);
```

Create a response
```csharp
public record ExampleResponse(int Id);
```

Create an endpoint
```csharp
public class Example : Endpoint<ExampleRequest, ExampleResponse>
{
    private readonly IService service;

    // Dependency Injection via constructor
    public Example(IService service)
    {
        this.service = service;
    }
    
    // Configure the Endpoint using Minimal API Route Builder. E.G
    // - Add Swagger Documentation
    // - Add Endpoint Filters
    // - Add Authorization
    public override void Configure(RouteHandlerBuilder configuration) => configuration
        .WithSummary("Example summary")
        .WithDescription("Example description");
    
    // Implement the request handler
    public override async Task<OneOf<ExampleResponse, Unathorized, BadRequest>> Handle(ExampleRequest request, CancellationToken cancellationToken)
    {
        // If any validation / authorization errors return them
        return new Unathorized();
        return new BadRequest("ErrorCode", "ErrorMessage")
        
        // Return the response
        return new ExampleResponse(request.Id);
    }
}
```

### Dependency Injection
Services can be injected into the `Endpoint` via standard constructor injection.
`Endpoints` are registered to the DI Container as `Scoped` services.

## Request Validation
Out of the box request validation is included using [FluentValidation.](https://github.com/FluentValidation/FluentValidation)
Request validators are registered automatically to the DI container.

```csharp
public class ExampleRequestValidator : AbstractValidator<ExampleRequest>
{
    public ExampleRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithErrorCode("ExampleErrorCode");
    }
}
```

## Endpoint Filters
Filters can be used to implement cross cutting concerns and create re-usable functionality across all or specific endpoints. 
It allows for **Pre and/or Post logic** to be implemented for each request.
Read [documentation here.](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/min-api-filters?view=aspnetcore-7.0)

### Creating an Endpoint Filter
```csharp
public class ExampleFilter<TRequest, TResponse> : EndpointFilter<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingFilter<TRequest, TResponse>> logger;
    
    // Dependency Injection via constructor
    public LoggingFilter(ILogger<LoggingFilter<TRequest, TResponse>> logger)
    {
        this.logger = logger;
    }

    protected override async Task<object?> Handle(TRequest request, EndpointFilterInvocationContext context, EndpointFilterDelegate next, CancellationToken cancellationToken)
    {
        logger.LogInformation("Before");
        var response = await next(context);
        logger.LogInformation("After");
        return response;
    }
}
```

### Add Endpoint Filter To Specific Endpoint(s)
Add the `EndpointFilter` via the `Configure(RouteHandlerBuilder configuration)` method on the `Endpoint`.
```csharp
public class Example : Endpoint<ExampleRequest, ExampleResponse>
{
    public override void Configure(RouteHandlerBuilder configuration) => configuration
        .AddEndpointFilter<ExampleFilter<ExampleRequest, ExampleResponse>>();
}
```

### Global Endpoint Filters
To add a global `EndpointFilter` which will run for **ALL** endpoints. Add the new global `EndpointFilter` in the following method on the [Base Endpoint Class]()
```csharp
public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
{
    return app.MapPost(GetType().Name, Execute)
        .WithOpenApi()
        .WithName(GetType().Name)
        .WithTags("All")
        .AddEndpointFilter<LoggingFilter<TRequest, TResponse>>()
        .AddEndpointFilter<UnhandledExceptionLoggingFilter<TRequest, TResponse>>()
        .AddEndpointFilter<ValidationFilter<TRequest, TResponse>>()
        .AddEndpointFilter<ExampleFilter<TRequest, TResponse>>() // Add this, or add it earlier in the chain if required
        .Produces<TResponse>()
        .Produces<BadRequest>(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);
}
```


#### Out of the Box Global Endpoint Filters
The following are out of the box `EndpointFilters` which are added to **ALL** endpoints by default
- [RequestLoggingFilter]() - Logs the request and execution time + response status code.
- [UnhandledExceptionFilter]() - Logs any unhandled exceptions and returns `500 Server Error`
- [ValidationFilter]() - Runs any request validators, if validation fails short circuits execution with a `400 Bad Request`
