using TekProvider.Domain.Enums;
using TekProvider.Domain.Exceptions;

namespace TekProvider.Domain.Entities;

public sealed class Customer
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string TaxId { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public CustomerStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public uint RowVersion { get; private set; }

    private Customer()
    {
    }

    public static Customer Create(string name, string taxId, string email, string? phone)
    {
        return new Customer
        {
            Id = Guid.NewGuid(),
            Name = name,
            TaxId = taxId,
            Email = email,
            Phone = phone,
            Status = CustomerStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string taxId, string email, string? phone)
    {
        Name = name;
        TaxId = taxId;
        Email = email;
        Phone = phone;
    }

    public void ChangeStatus(CustomerStatus newStatus, bool transitionAllowed)
    {
        if (!transitionAllowed)
        {
            throw new InvalidStateTransitionException(Status, newStatus);
        }

        Status = newStatus;
    }
}
