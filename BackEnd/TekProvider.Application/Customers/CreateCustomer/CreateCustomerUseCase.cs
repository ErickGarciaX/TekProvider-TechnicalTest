using FluentValidation;
using TekProvider.Application.Customers.Abstractions;
using TekProvider.Application.Customers.Dtos;
using TekProvider.Domain.Entities;
using TekProvider.Domain.Exceptions;

namespace TekProvider.Application.Customers.CreateCustomer;

public sealed class CreateCustomerUseCase : ICreateCustomerUseCase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IValidator<CreateCustomerCommand> _validator;

    public CreateCustomerUseCase(ICustomerRepository customerRepository, IValidator<CreateCustomerCommand> validator)
    {
        _customerRepository = customerRepository;
        _validator = validator;
    }

    public async Task<CustomerResponse> ExecuteAsync(CreateCustomerCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        if (await _customerRepository.ExistsByTaxIdAsync(command.TaxId, cancellationToken: cancellationToken))
        {
            throw new DuplicateCustomerException(nameof(Customer.TaxId));
        }

        if (await _customerRepository.ExistsByEmailAsync(command.Email, cancellationToken: cancellationToken))
        {
            throw new DuplicateCustomerException(nameof(Customer.Email));
        }

        var customer = Customer.Create(command.Name, command.TaxId, command.Email, command.Phone);

        _customerRepository.Add(customer);
        await _customerRepository.SaveChangesAsync(cancellationToken);

        return CustomerResponse.From(customer);
    }
}
