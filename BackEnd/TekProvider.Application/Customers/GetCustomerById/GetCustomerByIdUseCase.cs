using TekProvider.Application.Common.Exceptions;
using TekProvider.Application.Customers.Abstractions;
using TekProvider.Application.Customers.Dtos;

namespace TekProvider.Application.Customers.GetCustomerById;

public sealed class GetCustomerByIdUseCase : IGetCustomerByIdUseCase
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByIdUseCase(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerResponse> ExecuteAsync(GetCustomerByIdQuery query, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(query.Id, cancellationToken)
            ?? throw new CustomerNotFoundException(query.Id);

        return CustomerResponse.From(customer);
    }
}
