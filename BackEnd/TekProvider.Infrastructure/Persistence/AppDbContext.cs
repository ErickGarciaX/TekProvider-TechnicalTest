using Microsoft.EntityFrameworkCore;
using TekProvider.Domain.Entities;
using TekProvider.Infrastructure.Persistence.Entities;

namespace TekProvider.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<User> Users => Set<User>();
    public DbSet<CustomerStatusTransitionRule> CustomerStatusTransitions => Set<CustomerStatusTransitionRule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
