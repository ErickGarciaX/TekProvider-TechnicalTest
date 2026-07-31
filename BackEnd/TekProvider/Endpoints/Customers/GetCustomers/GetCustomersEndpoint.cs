using FastEndpoints;
using TekProvider.Application.Common;
using TekProvider.Application.Customers.Dtos;
using TekProvider.Application.Customers.GetCustomers;

namespace TekProvider.Endpoints.Customers.GetCustomers;

public sealed class GetCustomersEndpoint : Endpoint<GetCustomersRequest, PagedResult<CustomerResponse>>
{
    private readonly IGetCustomersUseCase _useCase;

    public GetCustomersEndpoint(IGetCustomersUseCase useCase)
    {
        _useCase = useCase;
    }

    public override void Configure()
    {
        Get("/api/customers");
    }

    public override async Task<PagedResult<CustomerResponse>> ExecuteAsync(GetCustomersRequest req, CancellationToken ct)
    {
        return await _useCase.ExecuteAsync(new GetCustomersQuery(req.Search, req.Page, req.PageSize), ct);
    }
}
