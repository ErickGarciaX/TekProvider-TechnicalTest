using TekProvider.Domain.Entities;
using TekProvider.Domain.Enums;

namespace TekProvider.Application.Customers.Dtos;

public sealed record CustomerResponse(
    Guid Id,
    string Name,
    string TaxId,
    string Email,
    string? Phone,
    CustomerStatus Status,
    DateTime CreatedAt,
    uint RowVersion)
{
    public static CustomerResponse From(Customer customer) => new(
        customer.Id,
        customer.Name,
        customer.TaxId,
        customer.Email,
        customer.Phone,
        customer.Status,
        customer.CreatedAt,
        customer.RowVersion);
}
