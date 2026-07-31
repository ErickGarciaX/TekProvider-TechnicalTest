using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TekProvider.Domain.Entities;

namespace TekProvider.Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
        builder.Property(c => c.TaxId).HasColumnName("TaxId").HasMaxLength(20).IsRequired();
        builder.Property(c => c.Email).HasColumnName("Email").HasMaxLength(200).IsRequired();
        builder.Property(c => c.Phone).HasMaxLength(20);
        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(c => c.CreatedAt).IsRequired();

        builder.Property(c => c.RowVersion)
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasIndex(c => c.Name);

        // Generated columns give a real case-insensitive uniqueness guarantee at the DB level,
        // not just an application-side check.
        builder.Property<string>("NormalizedTaxId")
            .HasMaxLength(20)
            .HasComputedColumnSql("lower(\"TaxId\")", stored: true);
        builder.HasIndex("NormalizedTaxId").IsUnique();

        builder.Property<string>("NormalizedEmail")
            .HasMaxLength(200)
            .HasComputedColumnSql("lower(\"Email\")", stored: true);
        builder.HasIndex("NormalizedEmail").IsUnique();
    }
}
