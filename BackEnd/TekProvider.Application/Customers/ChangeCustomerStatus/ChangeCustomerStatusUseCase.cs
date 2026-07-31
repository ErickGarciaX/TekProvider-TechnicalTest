using FluentValidation;
using TekProvider.Application.Common.Exceptions;
using TekProvider.Application.Customers.Abstractions;
using TekProvider.Application.Customers.Dtos;
using TekProvider.Domain.Policies;

namespace TekProvider.Application.Customers.ChangeCustomerStatus;

public sealed class ChangeCustomerStatusUseCase : IChangeCustomerStatusUseCase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerStatusTransitionPolicy _transitionPolicy;
    private readonly IValidator<ChangeCustomerStatusCommand> _validator;

    public ChangeCustomerStatusUseCase(
        ICustomerRepository customerRepository,
        ICustomerStatusTransitionPolicy transitionPolicy,
        IValidator<ChangeCustomerStatusCommand> validator)
    {
        _customerRepository = customerRepository;
        _transitionPolicy = transitionPolicy;
        _validator = validator;
    }

    public async Task<CustomerResponse> ExecuteAsync(ChangeCustomerStatusCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var customer = await _customerRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new CustomerNotFoundException(command.Id);

        var transitionAllowed = await _transitionPolicy.IsTransitionValidAsync(customer.Status, command.NewStatus, cancellationToken);
        customer.ChangeStatus(command.NewStatus, transitionAllowed);

        await _customerRepository.SaveChangesAsync(cancellationToken);

        return CustomerResponse.From(customer);
    }
}
