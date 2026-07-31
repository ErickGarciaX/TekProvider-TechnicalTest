using NSubstitute;
using TekProvider.Application.Common.Exceptions;
using TekProvider.Application.Customers.Abstractions;
using TekProvider.Application.Customers.ChangeCustomerStatus;
using TekProvider.Domain.Entities;
using TekProvider.Domain.Enums;
using TekProvider.Domain.Exceptions;
using TekProvider.Domain.Policies;

namespace TekProvider.Tests.Application.Customers;

public class ChangeCustomerStatusUseCaseTests
{
    private readonly ICustomerRepository _repository = Substitute.For<ICustomerRepository>();
    private readonly ICustomerStatusTransitionPolicy _policy = Substitute.For<ICustomerStatusTransitionPolicy>();
    private readonly ChangeCustomerStatusUseCase _useCase;

    public ChangeCustomerStatusUseCaseTests()
    {
        _useCase = new ChangeCustomerStatusUseCase(_repository, _policy, new ChangeCustomerStatusValidator());
    }

    [Fact]
    public async Task ExecuteAsync_WhenCustomerDoesNotExist_ThrowsCustomerNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((Customer?)null);

        await Assert.ThrowsAsync<CustomerNotFoundException>(
            () => _useCase.ExecuteAsync(new ChangeCustomerStatusCommand(id, CustomerStatus.Suspended)));
    }

    [Fact]
    public async Task ExecuteAsync_WhenPolicyAllowsTransition_UpdatesStatus()
    {
        var customer = Customer.Create("Acme", "TAX123", "acme@test.com", null);
        _repository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);
        _policy.IsTransitionValidAsync(CustomerStatus.Active, CustomerStatus.Suspended, Arg.Any<CancellationToken>()).Returns(true);

        var response = await _useCase.ExecuteAsync(new ChangeCustomerStatusCommand(customer.Id, CustomerStatus.Suspended));

        Assert.Equal(CustomerStatus.Suspended, response.Status);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPolicyDeniesTransition_ThrowsInvalidStateTransitionException()
    {
        var customer = Customer.Create("Acme", "TAX123", "acme@test.com", null);
        _repository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);
        _policy.IsTransitionValidAsync(CustomerStatus.Active, CustomerStatus.Suspended, Arg.Any<CancellationToken>()).Returns(false);

        await Assert.ThrowsAsync<InvalidStateTransitionException>(
            () => _useCase.ExecuteAsync(new ChangeCustomerStatusCommand(customer.Id, CustomerStatus.Suspended)));

        Assert.Equal(CustomerStatus.Active, customer.Status);
    }
}
