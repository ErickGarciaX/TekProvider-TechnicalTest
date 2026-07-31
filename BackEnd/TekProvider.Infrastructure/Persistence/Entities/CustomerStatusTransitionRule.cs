using TekProvider.Domain.Enums;

namespace TekProvider.Infrastructure.Persistence.Entities;

public sealed class CustomerStatusTransitionRule
{
    public CustomerStatus FromStatus { get; set; }
    public CustomerStatus ToStatus { get; set; }
    public bool IsActive { get; set; }
}
