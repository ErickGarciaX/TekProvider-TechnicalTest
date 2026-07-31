namespace TekProvider.Endpoints.Customers.GetCustomers;

public sealed class GetCustomersRequest
{
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
