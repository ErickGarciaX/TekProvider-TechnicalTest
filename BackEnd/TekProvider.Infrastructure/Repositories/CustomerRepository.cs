using Microsoft.EntityFrameworkCore;
using Npgsql;
using TekProvider.Application.Common.Exceptions;
using TekProvider.Application.Customers.Abstractions;
using TekProvider.Domain.Entities;
using TekProvider.Domain.Exceptions;
using TekProvider.Infrastructure.Persistence;

namespace TekProvider.Infrastructure.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _dbContext;

    public CustomerRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<Customer> Items, int TotalCount)> GetPagedAsync(
        string? search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Customers.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search}%";
            query = query.Where(c =>
                EF.Functions.ILike(c.Name, pattern) ||
                EF.Functions.ILike(c.TaxId, pattern) ||
                EF.Functions.ILike(c.Email, pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<bool> ExistsByTaxIdAsync(string taxId, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalized = taxId.ToLowerInvariant();
        var query = _dbContext.Customers.Where(c => EF.Property<string>(c, "NormalizedTaxId") == normalized);

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalized = email.ToLowerInvariant();
        var query = _dbContext.Customers.Where(c => EF.Property<string>(c, "NormalizedEmail") == normalized);

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public void Add(Customer customer) => _dbContext.Customers.Add(customer);

    public void SetConcurrencyToken(Customer customer, uint rowVersion) =>
        _dbContext.Entry(customer).Property(c => c.RowVersion).OriginalValue = rowVersion;

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyConflictException("The record was modified by another user.");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } pgEx)
        {
            var field = pgEx.ConstraintName switch
            {
                "IX_Customers_NormalizedTaxId" => nameof(Customer.TaxId),
                "IX_Customers_NormalizedEmail" => nameof(Customer.Email),
                _ => "Value"
            };

            throw new DuplicateCustomerException(field, pgEx.Detail ?? string.Empty);
        }
    }
}
