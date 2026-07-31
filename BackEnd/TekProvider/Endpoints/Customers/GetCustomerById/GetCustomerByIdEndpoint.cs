using FastEndpoints;
using TekProvider.Application.Customers.Dtos;
using TekProvider.Application.Customers.GetCustomerById;

namespace TekProvider.Endpoints.Customers.GetCustomerById;

public sealed class GetCustomerByIdEndpoint : Endpoint<GetCustomerByIdRequest, CustomerResponse>
{
    private readonly IGetCustomerByIdUseCase _useCase;

    public GetCustomerByIdEndpoint(IGetCustomerByIdUseCase useCase)
    {
        _useCase = useCase;
    }

    public override void Configure()
    {
        Get("/api/customers/{id}");
    }

    public override async Task<CustomerResponse> ExecuteAsync(GetCustomerByIdRequest req, CancellationToken ct)
    {
        return await _useCase.ExecuteAsync(new GetCustomerByIdQuery(req.Id), ct);
    }
}
