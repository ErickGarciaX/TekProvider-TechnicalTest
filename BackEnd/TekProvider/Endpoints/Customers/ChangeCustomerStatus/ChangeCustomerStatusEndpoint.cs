using FastEndpoints;
using TekProvider.Application.Customers.ChangeCustomerStatus;
using TekProvider.Application.Customers.Dtos;

namespace TekProvider.Endpoints.Customers.ChangeCustomerStatus;

public sealed class ChangeCustomerStatusEndpoint : Endpoint<ChangeCustomerStatusRequest, CustomerResponse>
{
    private readonly IChangeCustomerStatusUseCase _useCase;

    public ChangeCustomerStatusEndpoint(IChangeCustomerStatusUseCase useCase)
    {
        _useCase = useCase;
    }

    public override void Configure()
    {
        Patch("/api/customers/{id}/status");
    }

    public override async Task<CustomerResponse> ExecuteAsync(ChangeCustomerStatusRequest req, CancellationToken ct)
    {
        var command = new ChangeCustomerStatusCommand(req.Id, req.NewStatus);
        return await _useCase.ExecuteAsync(command, ct);
    }
}
