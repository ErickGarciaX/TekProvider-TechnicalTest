using TekProvider.Application.Customers.Dtos;

namespace TekProvider.Application.Customers.GetCustomerById;

public interface IGetCustomerByIdUseCase
{
    Task<CustomerResponse> ExecuteAsync(GetCustomerByIdQuery query, CancellationToken cancellationToken = default);
}
