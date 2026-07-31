namespace TekProvider.Application.Customers.CreateCustomer;

public sealed record CreateCustomerCommand(string Name, string TaxId, string Email, string? Phone);
