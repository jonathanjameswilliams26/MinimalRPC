namespace MinimalRPC.Endpoints;

public record ExampleRequest(int Id);
public record ExampleResponse(int Id);

public class ExampleRequestValidator : AbstractValidator<ExampleRequest>
{
    public ExampleRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithErrorCode("ExampleErrorCode");
    }
}

public class Example : Endpoint<ExampleRequest, ExampleResponse>
{
    public override void Configure(RouteHandlerBuilder configuration) => configuration
        .WithSummary("Example summary")
        .WithDescription("Example description");

    public override async Task<OneOf<ExampleResponse, Unathorized, BadRequest>> Handle(ExampleRequest request, CancellationToken cancellationToken)
    {
        return new ExampleResponse(request.Id);
    }
}