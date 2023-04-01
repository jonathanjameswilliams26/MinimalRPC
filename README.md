# MinimalRPC
An opinionated starter project using Minimal APIs with RPC style endpoints following the Request-Endpoint-Response (REPR) pattern.

## Opinionated Design Decisions
1. All endpoints are a `HTTP POST` with a `JSON Request Body`
2. Follows the [REPR Pattern](https://ardalis.com/mvc-controllers-are-dinosaurs-embrace-api-endpoints/) where each endpoint is its own class with its own request/response.
3. Only the following Status Codes are allowed: `200 OK`, `400 Bad Request`, '401 Unathorized', `500 Server Error`

## Why RPC over REST?
In my opinion and personal experience, RPC api's are:
- Easier to develop since everything is a `HTTP POST`, you only need to use work with a `JSON Request Body` and no longer need route params, query params etc.
- Easier to consume since routes are more descriptive like method names and everything required is in the body.

## Creating an Endpoint
The following code will register the endpoint `HTTP POST /Example`
```csharp

// Create a Request
public record ExampleRequest(int Id);

// Create a Response
public record ExampleResponse(int Id);

// Create the Endpoint
public class Example : Endpoint<ExampleRequest, ExampleResponse>
{
    // Configure the Endpoint using Minimal API Route Builder. E.G
    // - Add Swagger Documentation
    // - Add Endpoint Filters
    // - Add Authorization
    public override void Configure(RouteHandlerBuilder configuration) => configuration
        .WithSummary("Example summary")
        .WithDescription("Example description");

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
