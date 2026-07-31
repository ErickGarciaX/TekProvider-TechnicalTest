using TekProvider.Domain.Entities;

namespace TekProvider.Application.Auth.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    void Add(User user);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
