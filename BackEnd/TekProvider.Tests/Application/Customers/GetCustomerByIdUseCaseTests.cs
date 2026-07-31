using NSubstitute;
using TekProvider.Application.Common.Exceptions;
using TekProvider.Application.Customers.Abstractions;
using TekProvider.Application.Customers.GetCustomerById;
using TekProvider.Domain.Entities;

namespace TekProvider.Tests.Application.Customers;

public class GetCustomerByIdUseCaseTests
{
    private readonly ICustomerRepository _repository = Substitute.For<ICustomerRepository>();
    private readonly GetCustomerByIdUseCase _useCase;

    public GetCustomerByIdUseCaseTests()
    {
        _useCase = new GetCustomerByIdUseCase(_repository);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCustomerExists_ReturnsResponse()
    {
        var customer = Customer.Create("Acme", "TAX123", "acme@test.com", null);
        _repository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);

        var response = await _useCase.ExecuteAsync(new GetCustomerByIdQuery(customer.Id));

        Assert.Equal(customer.Id, response.Id);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCustomerDoesNotExist_ThrowsCustomerNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((Customer?)null);

        await Assert.ThrowsAsync<CustomerNotFoundException>(() => _useCase.ExecuteAsync(new GetCustomerByIdQuery(id)));
    }
}
