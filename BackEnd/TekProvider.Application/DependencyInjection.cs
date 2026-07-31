using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TekProvider.Application.Auth.Login;
using TekProvider.Application.Auth.Register;
using TekProvider.Application.Customers.ChangeCustomerStatus;
using TekProvider.Application.Customers.CreateCustomer;
using TekProvider.Application.Customers.GetCustomerById;
using TekProvider.Application.Customers.GetCustomers;
using TekProvider.Application.Customers.UpdateCustomer;

namespace TekProvider.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateCustomerValidator>();

        services.AddScoped<ICreateCustomerUseCase, CreateCustomerUseCase>();
        services.AddScoped<IUpdateCustomerUseCase, UpdateCustomerUseCase>();
        services.AddScoped<IChangeCustomerStatusUseCase, ChangeCustomerStatusUseCase>();
        services.AddScoped<IGetCustomerByIdUseCase, GetCustomerByIdUseCase>();
        services.AddScoped<IGetCustomersUseCase, GetCustomersUseCase>();
        services.AddScoped<ILoginUseCase, LoginUseCase>();
        services.AddScoped<IRegisterUseCase, RegisterUseCase>();

        return services;
    }
}
