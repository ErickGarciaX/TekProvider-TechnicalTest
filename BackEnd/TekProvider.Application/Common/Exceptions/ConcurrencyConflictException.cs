namespace TekProvider.Application.Common.Exceptions;

public sealed class ConcurrencyConflictException : AppException
{
    public ConcurrencyConflictException(string message)
        : base("concurrency-conflict", message)
    {
    }
}
