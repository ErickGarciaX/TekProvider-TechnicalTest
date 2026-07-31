namespace TekProvider.Endpoints.Customers.CreateCustomer;

public sealed class CreateCustomerRequest
{
    public string Name { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
}
