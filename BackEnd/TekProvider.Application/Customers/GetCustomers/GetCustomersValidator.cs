using FluentValidation;

namespace TekProvider.Application.Customers.GetCustomers;

public sealed class GetCustomersValidator : AbstractValidator<GetCustomersQuery>
{
    public GetCustomersValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
