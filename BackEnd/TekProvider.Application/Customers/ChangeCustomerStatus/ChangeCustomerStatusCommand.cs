using TekProvider.Domain.Enums;

namespace TekProvider.Application.Customers.ChangeCustomerStatus;

public sealed record ChangeCustomerStatusCommand(Guid Id, CustomerStatus NewStatus);
