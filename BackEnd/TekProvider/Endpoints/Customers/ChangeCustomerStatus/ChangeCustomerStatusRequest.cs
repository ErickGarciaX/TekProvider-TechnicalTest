using TekProvider.Domain.Enums;

namespace TekProvider.Endpoints.Customers.ChangeCustomerStatus;

public sealed class ChangeCustomerStatusRequest
{
    public Guid Id { get; set; }
    public CustomerStatus NewStatus { get; set; }
}
