namespace TekProvider.Application.Common.Exceptions;

public sealed class CustomerNotFoundException : AppException
{
    public Guid CustomerId { get; }

    public CustomerNotFoundException(Guid customerId)
        : base("customer.not-found", $"Customer '{customerId}' was not found.")
    {
        CustomerId = customerId;
    }
}
