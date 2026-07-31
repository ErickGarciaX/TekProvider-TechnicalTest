using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TekProvider.Domain.Enums;
using TekProvider.Infrastructure.Persistence.Entities;

namespace TekProvider.Infrastructure.Persistence.Configurations;

public sealed class CustomerStatusTransitionRuleConfiguration : IEntityTypeConfiguration<CustomerStatusTransitionRule>
{
    public void Configure(EntityTypeBuilder<CustomerStatusTransitionRule> builder)
    {
        builder.ToTable("CustomerStatusTransitions");

        builder.HasKey(r => new { r.FromStatus, r.ToStatus });

        builder.Property(r => r.FromStatus).HasConversion<string>().HasMaxLength(20);
        builder.Property(r => r.ToStatus).HasConversion<string>().HasMaxLength(20);
        builder.Property(r => r.IsActive).IsRequired();

        builder.HasData(
            new CustomerStatusTransitionRule { FromStatus = CustomerStatus.Active, ToStatus = CustomerStatus.Inactive, IsActive = true },
            new CustomerStatusTransitionRule { FromStatus = CustomerStatus.Inactive, ToStatus = CustomerStatus.Active, IsActive = true },
            new CustomerStatusTransitionRule { FromStatus = CustomerStatus.Active, ToStatus = CustomerStatus.Suspended, IsActive = true },
            new CustomerStatusTransitionRule { FromStatus = CustomerStatus.Suspended, ToStatus = CustomerStatus.Active, IsActive = true },
            new CustomerStatusTransitionRule { FromStatus = CustomerStatus.Inactive, ToStatus = CustomerStatus.Suspended, IsActive = false },
            new CustomerStatusTransitionRule { FromStatus = CustomerStatus.Suspended, ToStatus = CustomerStatus.Inactive, IsActive = false }
        );
    }
}
