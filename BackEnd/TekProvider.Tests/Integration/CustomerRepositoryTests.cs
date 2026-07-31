using Microsoft.EntityFrameworkCore;
using TekProvider.Application.Common.Exceptions;
using TekProvider.Domain.Entities;
using TekProvider.Domain.Exceptions;
using TekProvider.Infrastructure.Repositories;

namespace TekProvider.Tests.Integration;

[Collection("Postgres")]
public class CustomerRepositoryTests
{
    private readonly PostgresFixture _fixture;

    public CustomerRepositoryTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task SaveChangesAsync_WhenTaxIdDuplicatesCaseInsensitively_ThrowsDuplicateCustomerException()
    {
        var taxId = $"TAX{Guid.NewGuid():N}"[..12];

        await using (var setupContext = _fixture.CreateDbContext())
        {
            var setupRepository = new CustomerRepository(setupContext);
            setupRepository.Add(Customer.Create("First Co", taxId, $"{Guid.NewGuid():N}@test.com", null));
            await setupRepository.SaveChangesAsync();
        }

        await using var dbContext = _fixture.CreateDbContext();
        var repository = new CustomerRepository(dbContext);
        repository.Add(Customer.Create("Second Co", taxId.ToLowerInvariant(), $"{Guid.NewGuid():N}@test.com", null));

        await Assert.ThrowsAsync<DuplicateCustomerException>(() => repository.SaveChangesAsync());
    }

    [Fact]
    public async Task SaveChangesAsync_WhenEmailDuplicatesCaseInsensitively_ThrowsDuplicateCustomerException()
    {
        var email = $"{Guid.NewGuid():N}@Test.com";

        await using (var setupContext = _fixture.CreateDbContext())
        {
            var setupRepository = new CustomerRepository(setupContext);
            setupRepository.Add(Customer.Create("First Co", $"TAX{Guid.NewGuid():N}"[..12], email, null));
            await setupRepository.SaveChangesAsync();
        }

        await using var dbContext = _fixture.CreateDbContext();
        var repository = new CustomerRepository(dbContext);
        repository.Add(Customer.Create("Second Co", $"TAX{Guid.NewGuid():N}"[..12], email.ToLowerInvariant(), null));

        await Assert.ThrowsAsync<DuplicateCustomerException>(() => repository.SaveChangesAsync());
    }

    [Fact]
    public async Task SaveChangesAsync_WhenRowVersionIsStale_ThrowsConcurrencyConflictException()
    {
        Customer customer;
        uint originalRowVersion;

        await using (var setupContext = _fixture.CreateDbContext())
        {
            var setupRepository = new CustomerRepository(setupContext);
            customer = Customer.Create("Acme", $"TAX{Guid.NewGuid():N}"[..12], $"{Guid.NewGuid():N}@test.com", null);
            setupRepository.Add(customer);
            await setupRepository.SaveChangesAsync();
            originalRowVersion = customer.RowVersion;
        }

        // simulate another user updating the row first, which bumps Postgres' xmin
        await using (var otherContext = _fixture.CreateDbContext())
        {
            var otherCustomer = await otherContext.Customers.FirstAsync(c => c.Id == customer.Id);
            otherCustomer.Update("Acme Renamed", otherCustomer.TaxId, otherCustomer.Email, otherCustomer.Phone);
            await otherContext.SaveChangesAsync();
        }

        // now try to save using the row version the first client originally read
        await using var dbContext = _fixture.CreateDbContext();
        var repository = new CustomerRepository(dbContext);
        var staleCustomer = await dbContext.Customers.FirstAsync(c => c.Id == customer.Id);
        staleCustomer.Update("Acme From Stale Client", staleCustomer.TaxId, staleCustomer.Email, staleCustomer.Phone);
        repository.SetConcurrencyToken(staleCustomer, originalRowVersion);

        await Assert.ThrowsAsync<ConcurrencyConflictException>(() => repository.SaveChangesAsync());
    }
}
