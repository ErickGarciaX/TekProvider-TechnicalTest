using NSubstitute;
using TekProvider.Application.Customers.Abstractions;
using TekProvider.Application.Customers.CreateCustomer;
using TekProvider.Domain.Entities;
using TekProvider.Domain.Exceptions;

namespace TekProvider.Tests.Application.Customers;

public class CreateCustomerUseCaseTests
{
    private readonly ICustomerRepository _repository = Substitute.For<ICustomerRepository>();
    private readonly CreateCustomerUseCase _useCase;

    public CreateCustomerUseCaseTests()
    {
        _useCase = new CreateCustomerUseCase(_repository, new CreateCustomerValidator());
    }

    [Fact]
    public async Task ExecuteAsync_WithValidCommand_AddsCustomerAndReturnsResponse()
    {
        var command = new CreateCustomerCommand("Acme Corp", "TAX123", "acme@test.com", "555-1234");

        var response = await _useCase.ExecuteAsync(command);

        Assert.Equal("Acme Corp", response.Name);
        _repository.Received(1).Add(Arg.Is<Customer>(c => c!.TaxId == "TAX123"));
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenTaxIdAlreadyExists_ThrowsDuplicateCustomerException()
    {
        _repository.ExistsByTaxIdAsync("TAX123", null, Arg.Any<CancellationToken>()).Returns(true);
        var command = new CreateCustomerCommand("Acme Corp", "TAX123", "acme@test.com", null);

        await Assert.ThrowsAsync<DuplicateCustomerException>(() => _useCase.ExecuteAsync(command));

        _repository.DidNotReceive().Add(Arg.Any<Customer>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenEmailAlreadyExists_ThrowsDuplicateCustomerException()
    {
        _repository.ExistsByEmailAsync("acme@test.com", null, Arg.Any<CancellationToken>()).Returns(true);
        var command = new CreateCustomerCommand("Acme Corp", "TAX123", "acme@test.com", null);

        await Assert.ThrowsAsync<DuplicateCustomerException>(() => _useCase.ExecuteAsync(command));
    }

    [Theory]
    [InlineData("", "TAX123", "acme@test.com")]
    [InlineData("Acme Corp", "", "acme@test.com")]
    [InlineData("Acme Corp", "TAX123", "not-an-email")]
    public async Task ExecuteAsync_WithInvalidCommand_ThrowsValidationException(string name, string taxId, string email)
    {
        var command = new CreateCustomerCommand(name, taxId, email, null);

        await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _useCase.ExecuteAsync(command));
    }
}
