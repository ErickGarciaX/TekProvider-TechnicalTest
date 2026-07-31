namespace TekProvider.Application.Common.Exceptions;

public sealed class InvalidCredentialsException : AppException
{
    public InvalidCredentialsException()
        : base("auth.invalid-credentials", "Username or password is incorrect.")
    {
    }
}
