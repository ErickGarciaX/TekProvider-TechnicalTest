using TekProvider.Application.Customers.Dtos;

namespace TekProvider.Application.Customers.CreateCustomer;

public interface ICreateCustomerUseCase
{
    Task<CustomerResponse> ExecuteAsync(CreateCustomerCommand command, CancellationToken cancellationToken = default);
}
