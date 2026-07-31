using FluentValidation;
using TekProvider.Application.Auth.Abstractions;
using TekProvider.Application.Auth.Dtos;
using TekProvider.Application.Common.Exceptions;
using TekProvider.Domain.Entities;

namespace TekProvider.Application.Auth.Register;

public sealed class RegisterUseCase : IRegisterUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IValidator<RegisterCommand> _validator;

    public RegisterUseCase(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator,
        IValidator<RegisterCommand> validator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _validator = validator;
    }

    public async Task<LoginResponse> ExecuteAsync(RegisterCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        if (await _userRepository.GetByUsernameAsync(command.Username, cancellationToken) is not null)
        {
            throw new UsernameAlreadyTakenException(command.Username);
        }

        var user = User.Create(command.Username, _passwordHasher.Hash(command.Password));

        _userRepository.Add(user);
        await _userRepository.SaveChangesAsync(cancellationToken);

        var (token, expiresAtUtc) = _tokenGenerator.GenerateToken(user);

        return new LoginResponse(token, expiresAtUtc, user.Username);
    }
}
