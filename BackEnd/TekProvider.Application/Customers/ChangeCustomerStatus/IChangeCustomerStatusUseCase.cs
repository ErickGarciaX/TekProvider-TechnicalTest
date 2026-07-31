using TekProvider.Application.Customers.Dtos;

namespace TekProvider.Application.Customers.ChangeCustomerStatus;

public interface IChangeCustomerStatusUseCase
{
    Task<CustomerResponse> ExecuteAsync(ChangeCustomerStatusCommand command, CancellationToken cancellationToken = default);
}
