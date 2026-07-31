namespace TekProvider.Application.Customers.UpdateCustomer;

public sealed record UpdateCustomerCommand(Guid Id, string Name, string TaxId, string Email, string? Phone, uint RowVersion);
