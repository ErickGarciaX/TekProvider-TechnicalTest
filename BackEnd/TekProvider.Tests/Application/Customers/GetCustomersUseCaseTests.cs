using NSubstitute;
using TekProvider.Application.Customers.Abstractions;
using TekProvider.Application.Customers.GetCustomers;
using TekProvider.Domain.Entities;

namespace TekProvider.Tests.Application.Customers;

public class GetCustomersUseCaseTests
{
    private readonly ICustomerRepository _repository = Substitute.For<ICustomerRepository>();
    private readonly GetCustomersUseCase _useCase;

    public GetCustomersUseCaseTests()
    {
        _useCase = new GetCustomersUseCase(_repository, new GetCustomersValidator());
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsPagedResultFromRepository()
    {
        var customers = new List<Customer> { Customer.Create("Acme", "TAX123", "acme@test.com", null) };
        _repository.GetPagedAsync("Acme", 1, 20, Arg.Any<CancellationToken>()).Returns((customers, 1));

        var result = await _useCase.ExecuteAsync(new GetCustomersQuery("Acme", 1, 20));

        Assert.Single(result.Items);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PageSize);
    }

    [Theory]
    [InlineData(0, 20)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    public async Task ExecuteAsync_WithInvalidPaging_ThrowsValidationException(int page, int pageSize)
    {
        await Assert.ThrowsAsync<FluentValidation.ValidationException>(
            () => _useCase.ExecuteAsync(new GetCustomersQuery(null, page, pageSize)));
    }
}
