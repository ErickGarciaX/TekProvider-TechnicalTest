namespace TekProvider.Endpoints.Customers.UpdateCustomer;

public sealed class UpdateCustomerRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public uint RowVersion { get; set; }
}
