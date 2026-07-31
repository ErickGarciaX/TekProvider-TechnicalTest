namespace TekProvider.Application.Common.Exceptions;

public sealed class UsernameAlreadyTakenException : AppException
{
    public UsernameAlreadyTakenException(string username)
        : base("auth.username-taken", $"Username '{username}' is already taken.")
    {
    }
}
