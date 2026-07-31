using Microsoft.EntityFrameworkCore;
using TekProvider.Domain.Enums;
using TekProvider.Domain.Policies;
using TekProvider.Infrastructure.Persistence;

namespace TekProvider.Infrastructure.Policies;

public sealed class CustomerStatusTransitionPolicy : ICustomerStatusTransitionPolicy
{
    private readonly AppDbContext _dbContext;

    public CustomerStatusTransitionPolicy(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsTransitionValidAsync(CustomerStatus fromStatus, CustomerStatus toStatus, CancellationToken cancellationToken = default)
    {
        if (fromStatus == toStatus)
        {
            return false;
        }

        return await _dbContext.CustomerStatusTransitions.AsNoTracking().AnyAsync(
            r => r.FromStatus == fromStatus && r.ToStatus == toStatus && r.IsActive,
            cancellationToken);
    }
}
