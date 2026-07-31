using TekProvider.Domain.Entities;

namespace TekProvider.Application.Auth.Abstractions;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAtUtc) GenerateToken(User user);
}
