namespace JobTracker.Application.Common.Models;

public class Result
{
    public bool Succeeded { get; init; }
    public IEnumerable<string> Errors { get; init; } = [];

    public static Result Success() => new() { Succeeded = true };
    public static Result Failure(IEnumerable<string> errors) => new() { Succeeded = false, Errors = errors };
    public static Result Failure(string error) => Failure([error]);
}

public class Result<T> : Result
{
    public T? Value { get; init; }

    public static Result<T> Success(T value) => new() { Succeeded = true, Value = value };
    public new static Result<T> Failure(IEnumerable<string> errors) => new() { Succeeded = false, Errors = errors };
    public new static Result<T> Failure(string error) => Failure([error]);
}
