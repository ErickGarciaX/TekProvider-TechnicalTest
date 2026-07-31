using FastEndpoints;
using TekProvider.Application.Auth.Dtos;
using TekProvider.Application.Auth.Login;

namespace TekProvider.Endpoints.Auth.Login;

public sealed class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly ILoginUseCase _useCase;

    public LoginEndpoint(ILoginUseCase useCase)
    {
        _useCase = useCase;
    }

    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
    }

    public override async Task<LoginResponse> ExecuteAsync(LoginRequest req, CancellationToken ct)
    {
        return await _useCase.ExecuteAsync(new LoginCommand(req.Username, req.Password), ct);
    }
}
