using FastEndpoints;
using TekProvider.Application.Customers.CreateCustomer;
using TekProvider.Application.Customers.Dtos;

namespace TekProvider.Endpoints.Customers.CreateCustomer;

public sealed class CreateCustomerEndpoint : Endpoint<CreateCustomerRequest, CustomerResponse>
{
    private readonly ICreateCustomerUseCase _useCase;

    public CreateCustomerEndpoint(ICreateCustomerUseCase useCase)
    {
        _useCase = useCase;
    }

    public override void Configure()
    {
        Post("/api/customers");
    }

    public override async Task<CustomerResponse> ExecuteAsync(CreateCustomerRequest req, CancellationToken ct)
    {
        var response = await _useCase.ExecuteAsync(new CreateCustomerCommand(req.Name, req.TaxId, req.Email, req.Phone), ct);
        HttpContext.Response.StatusCode = StatusCodes.Status201Created;
        return response;
    }
}
