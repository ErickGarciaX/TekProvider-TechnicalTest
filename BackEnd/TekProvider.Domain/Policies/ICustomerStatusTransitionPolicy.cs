using TekProvider.Domain.Enums;

namespace TekProvider.Domain.Policies;

public interface ICustomerStatusTransitionPolicy
{
    Task<bool> IsTransitionValidAsync(CustomerStatus fromStatus, CustomerStatus toStatus, CancellationToken cancellationToken = default);
}
