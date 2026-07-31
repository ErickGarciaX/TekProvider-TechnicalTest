namespace TekProvider.Application.Customers.GetCustomers;

public sealed record GetCustomersQuery(string? Search, int Page, int PageSize);
