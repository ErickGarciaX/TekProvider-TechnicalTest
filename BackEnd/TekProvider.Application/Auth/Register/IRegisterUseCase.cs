using TekProvider.Application.Auth.Dtos;

namespace TekProvider.Application.Auth.Register;

public interface IRegisterUseCase
{
    Task<LoginResponse> ExecuteAsync(RegisterCommand command, CancellationToken cancellationToken = default);
}
