using TekProvider.Application.Common;
using TekProvider.Application.Customers.Dtos;

namespace TekProvider.Application.Customers.GetCustomers;

public interface IGetCustomersUseCase
{
    Task<PagedResult<CustomerResponse>> ExecuteAsync(GetCustomersQuery query, CancellationToken cancellationToken = default);
}
