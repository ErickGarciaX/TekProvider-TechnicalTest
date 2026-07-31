using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TekProvider.Application.Auth.Abstractions;
using TekProvider.Domain.Entities;

namespace TekProvider.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        if (await dbContext.Users.AnyAsync())
        {
            return;
        }

        var testUser = User.Create("admin", passwordHasher.Hash("Admin123!"));
        dbContext.Users.Add(testUser);
        await dbContext.SaveChangesAsync();
    }
}
