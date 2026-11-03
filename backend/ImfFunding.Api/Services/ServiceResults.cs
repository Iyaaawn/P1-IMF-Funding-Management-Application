// a small generic class so the service can say what kind of failure happened, 
// and the controller can turn it into the right status code. This is your “generics” example for interviews.

namespace ImfFunding.Api.Services;

public enum ServiceErrorType { None, NotFound, Validation, Conflict }

public class ServiceResult<T>
{
    public T? Value { get; private init; }
    public ServiceErrorType Error { get; private init; }
    public string? Message { get; private init; }
    public bool Succeeded => Error == ServiceErrorType.None;

    public static ServiceResult<T> Ok(T value) => new() { Value = value };
    public static ServiceResult<T> NotFound(string message) =>
        new() { Error = ServiceErrorType.NotFound, Message = message };
    public static ServiceResult<T> Invalid(string message) =>
        new() { Error = ServiceErrorType.Validation, Message = message };
    public static ServiceResult<T> Conflict(string message) =>
        new() { Error = ServiceErrorType.Conflict, Message = message };
}