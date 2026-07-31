using TekProvider.Application.Auth.Dtos;

namespace TekProvider.Application.Auth.Login;

public interface ILoginUseCase
{
    Task<LoginResponse> ExecuteAsync(LoginCommand command, CancellationToken cancellationToken = default);
}
