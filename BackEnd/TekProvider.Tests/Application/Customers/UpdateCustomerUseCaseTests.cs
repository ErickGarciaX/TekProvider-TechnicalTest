using NSubstitute;
using TekProvider.Application.Common.Exceptions;
using TekProvider.Application.Customers.Abstractions;
using TekProvider.Application.Customers.UpdateCustomer;
using TekProvider.Domain.Entities;
using TekProvider.Domain.Exceptions;

namespace TekProvider.Tests.Application.Customers;

public class UpdateCustomerUseCaseTests
{
    private readonly ICustomerRepository _repository = Substitute.For<ICustomerRepository>();
    private readonly UpdateCustomerUseCase _useCase;

    public UpdateCustomerUseCaseTests()
    {
        _useCase = new UpdateCustomerUseCase(_repository, new UpdateCustomerValidator());
    }

    [Fact]
    public async Task ExecuteAsync_WhenCustomerDoesNotExist_ThrowsCustomerNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((Customer?)null);
        var command = new UpdateCustomerCommand(id, "Acme", "TAX123", "acme@test.com", null, 1);

        await Assert.ThrowsAsync<CustomerNotFoundException>(() => _useCase.ExecuteAsync(command));
    }

    [Fact]
    public async Task ExecuteAsync_WhenTaxIdTakenByAnotherCustomer_ThrowsDuplicateCustomerException()
    {
        var customer = Customer.Create("Acme", "TAX123", "acme@test.com", null);
        _repository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);
        _repository.ExistsByTaxIdAsync("TAX999", customer.Id, Arg.Any<CancellationToken>()).Returns(true);

        var command = new UpdateCustomerCommand(customer.Id, "Acme", "TAX999", "acme@test.com", null, 1);

        await Assert.ThrowsAsync<DuplicateCustomerException>(() => _useCase.ExecuteAsync(command));
    }

    [Fact]
    public async Task ExecuteAsync_WithValidCommand_UpdatesCustomerAndSetsConcurrencyToken()
    {
        var customer = Customer.Create("Acme", "TAX123", "acme@test.com", null);
        _repository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);

        var command = new UpdateCustomerCommand(customer.Id, "Acme Updated", "TAX123", "acme@test.com", "555-0000", 42);

        var response = await _useCase.ExecuteAsync(command);

        Assert.Equal("Acme Updated", response.Name);
        _repository.Received(1).SetConcurrencyToken(customer, 42);
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
