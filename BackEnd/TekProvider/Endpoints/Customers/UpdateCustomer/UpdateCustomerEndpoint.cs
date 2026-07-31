using FastEndpoints;
using TekProvider.Application.Customers.Dtos;
using TekProvider.Application.Customers.UpdateCustomer;

namespace TekProvider.Endpoints.Customers.UpdateCustomer;

public sealed class UpdateCustomerEndpoint : Endpoint<UpdateCustomerRequest, CustomerResponse>
{
    private readonly IUpdateCustomerUseCase _useCase;

    public UpdateCustomerEndpoint(IUpdateCustomerUseCase useCase)
    {
        _useCase = useCase;
    }

    public override void Configure()
    {
        Put("/api/customers/{id}");
    }

    public override async Task<CustomerResponse> ExecuteAsync(UpdateCustomerRequest req, CancellationToken ct)
    {
        var command = new UpdateCustomerCommand(req.Id, req.Name, req.TaxId, req.Email, req.Phone, req.RowVersion);
        return await _useCase.ExecuteAsync(command, ct);
    }
}
