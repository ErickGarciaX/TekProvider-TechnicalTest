using TekProvider.Domain.Enums;

namespace TekProvider.Domain.Exceptions;

public sealed class InvalidStateTransitionException : DomainException
{
    public CustomerStatus FromStatus { get; }
    public CustomerStatus ToStatus { get; }

    public InvalidStateTransitionException(CustomerStatus fromStatus, CustomerStatus toStatus)
        : base("customer.invalid-status-transition", $"Transition from '{fromStatus}' to '{toStatus}' is not allowed.")
    {
        FromStatus = fromStatus;
        ToStatus = toStatus;
    }
}
