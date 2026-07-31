using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TekProvider.Application.Auth.Abstractions;
using TekProvider.Application.Customers.Abstractions;
using TekProvider.Domain.Policies;
using TekProvider.Infrastructure.Auth;
using TekProvider.Infrastructure.Persistence;
using TekProvider.Infrastructure.Policies;
using TekProvider.Infrastructure.Repositories;

namespace TekProvider.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICustomerStatusTransitionPolicy, CustomerStatusTransitionPolicy>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
