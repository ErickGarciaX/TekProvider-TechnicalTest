using FluentValidation;
using TekProvider.Application.Common.Exceptions;
using TekProvider.Application.Customers.Abstractions;
using TekProvider.Application.Customers.Dtos;
using TekProvider.Domain.Entities;
using TekProvider.Domain.Exceptions;

namespace TekProvider.Application.Customers.UpdateCustomer;

public sealed class UpdateCustomerUseCase : IUpdateCustomerUseCase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IValidator<UpdateCustomerCommand> _validator;

    public UpdateCustomerUseCase(ICustomerRepository customerRepository, IValidator<UpdateCustomerCommand> validator)
    {
        _customerRepository = customerRepository;
        _validator = validator;
    }

    public async Task<CustomerResponse> ExecuteAsync(UpdateCustomerCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var customer = await _customerRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new CustomerNotFoundException(command.Id);

        if (await _customerRepository.ExistsByTaxIdAsync(command.TaxId, command.Id, cancellationToken))
        {
            throw new DuplicateCustomerException(nameof(Customer.TaxId), command.TaxId);
        }

        if (await _customerRepository.ExistsByEmailAsync(command.Email, command.Id, cancellationToken))
        {
            throw new DuplicateCustomerException(nameof(Customer.Email), command.Email);
        }

        customer.Update(command.Name, command.TaxId, command.Email, command.Phone);
        _customerRepository.SetConcurrencyToken(customer, command.RowVersion);
        await _customerRepository.SaveChangesAsync(cancellationToken);

        return CustomerResponse.From(customer);
    }
}
