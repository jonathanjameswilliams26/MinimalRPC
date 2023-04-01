namespace MinimalRPC.Endpoints.Common;

public static class SensitiveRequests
{
    private static readonly HashSet<Type> types = new();
    public static bool CanLogRequestBody(object request) => !types.Contains(request.GetType());
}
