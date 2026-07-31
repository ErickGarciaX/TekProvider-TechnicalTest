using FluentValidation;
using TekProvider.Application.Auth.Abstractions;
using TekProvider.Application.Auth.Dtos;
using TekProvider.Application.Common.Exceptions;

namespace TekProvider.Application.Auth.Login;

public sealed class LoginUseCase : ILoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IValidator<LoginCommand> _validator;

    public LoginUseCase(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator,
        IValidator<LoginCommand> validator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _validator = validator;
    }

    public async Task<LoginResponse> ExecuteAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var user = await _userRepository.GetByUsernameAsync(command.Username, cancellationToken)
            ?? throw new InvalidCredentialsException();

        if (!_passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException();
        }

        var (token, expiresAtUtc) = _tokenGenerator.GenerateToken(user);

        return new LoginResponse(token, expiresAtUtc, user.Username);
    }
}
