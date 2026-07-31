using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TekProvider.Domain.Entities;

namespace TekProvider.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username).HasColumnName("Username").HasMaxLength(100).IsRequired();
        builder.Property(u => u.PasswordHash).HasMaxLength(500).IsRequired();
        builder.Property(u => u.CreatedAt).IsRequired();

        builder.Property<string>("NormalizedUsername")
            .HasMaxLength(100)
            .HasComputedColumnSql("lower(\"Username\")", stored: true);
        builder.HasIndex("NormalizedUsername").IsUnique();
    }
}
