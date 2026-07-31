using NSubstitute;
using TekProvider.Application.Auth.Abstractions;
using TekProvider.Application.Auth.Login;
using TekProvider.Application.Common.Exceptions;
using TekProvider.Domain.Entities;

namespace TekProvider.Tests.Application.Auth;

public class LoginUseCaseTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtTokenGenerator _tokenGenerator = Substitute.For<IJwtTokenGenerator>();
    private readonly LoginUseCase _useCase;

    public LoginUseCaseTests()
    {
        _useCase = new LoginUseCase(_userRepository, _passwordHasher, _tokenGenerator, new LoginValidator());
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserDoesNotExist_ThrowsInvalidCredentialsException()
    {
        _userRepository.GetByUsernameAsync("erick", Arg.Any<CancellationToken>()).Returns((User?)null);

        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => _useCase.ExecuteAsync(new LoginCommand("erick", "Passw0rd!")));
    }

    [Fact]
    public async Task ExecuteAsync_WhenPasswordIsWrong_ThrowsInvalidCredentialsException()
    {
        var user = User.Create("erick", "stored-hash");
        _userRepository.GetByUsernameAsync("erick", Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify("wrong-password", "stored-hash").Returns(false);

        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => _useCase.ExecuteAsync(new LoginCommand("erick", "wrong-password")));
    }

    [Fact]
    public async Task ExecuteAsync_WithValidCredentials_ReturnsToken()
    {
        var user = User.Create("erick", "stored-hash");
        _userRepository.GetByUsernameAsync("erick", Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify("Passw0rd!", "stored-hash").Returns(true);
        _tokenGenerator.GenerateToken(user).Returns(("jwt-token", DateTime.UtcNow.AddHours(1)));

        var response = await _useCase.ExecuteAsync(new LoginCommand("erick", "Passw0rd!"));

        Assert.Equal("jwt-token", response.Token);
        Assert.Equal("erick", response.Username);
    }
}
