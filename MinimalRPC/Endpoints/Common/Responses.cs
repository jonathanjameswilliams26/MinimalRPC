using FluentValidation.Results;

namespace MinimalRPC.Endpoints.Common;

public record struct Unathorized;
public record struct BadRequest
{
    public string Code { get; set; }
    public string Message { get; set; }

    public BadRequest(string code, string message)
    {
        Code = code ?? "N/A";
        Message = message ?? "Bad request";
    }

    public BadRequest(ValidationFailure failure) : this(failure.ErrorCode, failure.ErrorMessage) { }
}
