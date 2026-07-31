using FastEndpoints;
using TekProvider.Application.Auth.Dtos;
using TekProvider.Application.Auth.Register;

namespace TekProvider.Endpoints.Auth.Register;

public sealed class RegisterEndpoint : Endpoint<RegisterRequest, LoginResponse>
{
    private readonly IRegisterUseCase _useCase;

    public RegisterEndpoint(IRegisterUseCase useCase)
    {
        _useCase = useCase;
    }

    public override void Configure()
    {
        Post("/api/auth/register");
        AllowAnonymous();
    }

    public override async Task<LoginResponse> ExecuteAsync(RegisterRequest req, CancellationToken ct)
    {
        var response = await _useCase.ExecuteAsync(new RegisterCommand(req.Username, req.Password), ct);
        HttpContext.Response.StatusCode = StatusCodes.Status201Created;
        return response;
    }
}
