using NSubstitute;
using TekProvider.Application.Auth.Abstractions;
using TekProvider.Application.Auth.Register;
using TekProvider.Application.Common.Exceptions;
using TekProvider.Domain.Entities;

namespace TekProvider.Tests.Application.Auth;

public class RegisterUseCaseTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtTokenGenerator _tokenGenerator = Substitute.For<IJwtTokenGenerator>();
    private readonly RegisterUseCase _useCase;

    public RegisterUseCaseTests()
    {
        _useCase = new RegisterUseCase(_userRepository, _passwordHasher, _tokenGenerator, new RegisterValidator());
    }

    [Fact]
    public async Task ExecuteAsync_WhenUsernameAlreadyTaken_ThrowsUsernameAlreadyTakenException()
    {
        _userRepository.GetByUsernameAsync("erick", Arg.Any<CancellationToken>()).Returns(User.Create("erick", "hash"));

        await Assert.ThrowsAsync<UsernameAlreadyTakenException>(
            () => _useCase.ExecuteAsync(new RegisterCommand("erick", "Passw0rd!")));
    }

    [Fact]
    public async Task ExecuteAsync_WithValidCommand_CreatesUserAndReturnsToken()
    {
        _userRepository.GetByUsernameAsync("erick", Arg.Any<CancellationToken>()).Returns((User?)null);
        _passwordHasher.Hash("Passw0rd!").Returns("hashed");
        _tokenGenerator.GenerateToken(Arg.Any<User>()).Returns(("jwt-token", DateTime.UtcNow.AddHours(1)));

        var response = await _useCase.ExecuteAsync(new RegisterCommand("erick", "Passw0rd!"));

        Assert.Equal("jwt-token", response.Token);
        Assert.Equal("erick", response.Username);
        _userRepository.Received(1).Add(Arg.Is<User>(u => u!.Username == "erick" && u.PasswordHash == "hashed"));
        await _userRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithShortPassword_ThrowsValidationException()
    {
        await Assert.ThrowsAsync<FluentValidation.ValidationException>(
            () => _useCase.ExecuteAsync(new RegisterCommand("erick", "short")));
    }
}
