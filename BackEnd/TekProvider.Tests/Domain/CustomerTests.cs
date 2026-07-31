using TekProvider.Domain.Entities;
using TekProvider.Domain.Enums;
using TekProvider.Domain.Exceptions;

namespace TekProvider.Tests.Domain;

public class CustomerTests
{
    [Fact]
    public void Create_SetsExpectedDefaults()
    {
        var customer = Customer.Create("Acme Corp", "TAX123", "acme@test.com", "555-1234");

        Assert.NotEqual(Guid.Empty, customer.Id);
        Assert.Equal("Acme Corp", customer.Name);
        Assert.Equal("TAX123", customer.TaxId);
        Assert.Equal("acme@test.com", customer.Email);
        Assert.Equal("555-1234", customer.Phone);
        Assert.Equal(CustomerStatus.Active, customer.Status);
    }

    [Fact]
    public void Update_ReplacesEditableFields()
    {
        var customer = Customer.Create("Acme Corp", "TAX123", "acme@test.com", null);

        customer.Update("Acme Corp Updated", "TAX999", "new@test.com", "555-0000");

        Assert.Equal("Acme Corp Updated", customer.Name);
        Assert.Equal("TAX999", customer.TaxId);
        Assert.Equal("new@test.com", customer.Email);
        Assert.Equal("555-0000", customer.Phone);
    }

    [Fact]
    public void ChangeStatus_WhenTransitionAllowed_UpdatesStatus()
    {
        var customer = Customer.Create("Acme Corp", "TAX123", "acme@test.com", null);

        customer.ChangeStatus(CustomerStatus.Suspended, transitionAllowed: true);

        Assert.Equal(CustomerStatus.Suspended, customer.Status);
    }

    [Fact]
    public void ChangeStatus_WhenTransitionNotAllowed_ThrowsInvalidStateTransitionException()
    {
        var customer = Customer.Create("Acme Corp", "TAX123", "acme@test.com", null);

        var exception = Assert.Throws<InvalidStateTransitionException>(
            () => customer.ChangeStatus(CustomerStatus.Suspended, transitionAllowed: false));

        Assert.Equal(CustomerStatus.Active, exception.FromStatus);
        Assert.Equal(CustomerStatus.Suspended, exception.ToStatus);
        Assert.Equal(CustomerStatus.Active, customer.Status);
    }
}
