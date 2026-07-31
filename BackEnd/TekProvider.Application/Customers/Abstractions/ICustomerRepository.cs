using TekProvider.Domain.Entities;

namespace TekProvider.Application.Customers.Abstractions;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Customer> Items, int TotalCount)> GetPagedAsync(
        string? search, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<bool> ExistsByTaxIdAsync(string taxId, Guid? excludeId = null, CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default);

    void Add(Customer customer);

    void SetConcurrencyToken(Customer customer, uint rowVersion);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
