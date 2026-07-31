namespace TekProvider.Application.Auth.Dtos;

public sealed record LoginResponse(string Token, DateTime ExpiresAtUtc, string Username);
