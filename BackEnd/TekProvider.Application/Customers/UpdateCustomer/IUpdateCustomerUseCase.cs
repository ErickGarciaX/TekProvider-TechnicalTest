using TekProvider.Application.Customers.Dtos;

namespace TekProvider.Application.Customers.UpdateCustomer;

public interface IUpdateCustomerUseCase
{
    Task<CustomerResponse> ExecuteAsync(UpdateCustomerCommand command, CancellationToken cancellationToken = default);
}
