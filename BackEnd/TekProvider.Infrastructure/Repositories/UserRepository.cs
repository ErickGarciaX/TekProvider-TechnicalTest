using Microsoft.EntityFrameworkCore;
using TekProvider.Application.Auth.Abstractions;
using TekProvider.Domain.Entities;
using TekProvider.Infrastructure.Persistence;

namespace TekProvider.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var normalized = username.ToLowerInvariant();
        return _dbContext.Users.FirstOrDefaultAsync(u => EF.Property<string>(u, "NormalizedUsername") == normalized, cancellationToken);
    }

    public void Add(User user) => _dbContext.Users.Add(user);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _dbContext.SaveChangesAsync(cancellationToken);
}
