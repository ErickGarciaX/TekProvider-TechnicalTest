namespace TekProvider.Domain.Exceptions;

public sealed class DuplicateCustomerException : DomainException
{
    public string Field { get; }

    public DuplicateCustomerException(string field)
        : base("customer.duplicate", $"A customer with this {field} already exists.")
    {
        Field = field;
    }
}
