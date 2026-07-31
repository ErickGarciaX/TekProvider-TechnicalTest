namespace TekProvider.Domain.Exceptions;

public sealed class DuplicateCustomerException : DomainException
{
    public string Field { get; }
    public string Value { get; }

    public DuplicateCustomerException(string field, string value)
        : base("customer.duplicate", $"A customer with {field} '{value}' already exists.")
    {
        Field = field;
        Value = value;
    }
}
