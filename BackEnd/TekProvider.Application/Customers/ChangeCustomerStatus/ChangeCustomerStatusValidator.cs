using FluentValidation;

namespace TekProvider.Application.Customers.ChangeCustomerStatus;

public sealed class ChangeCustomerStatusValidator : AbstractValidator<ChangeCustomerStatusCommand>
{
    public ChangeCustomerStatusValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.NewStatus).IsInEnum();
    }
}
