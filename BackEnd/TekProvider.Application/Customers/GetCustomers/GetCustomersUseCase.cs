using FluentValidation;
using TekProvider.Application.Common;
using TekProvider.Application.Customers.Abstractions;
using TekProvider.Application.Customers.Dtos;

namespace TekProvider.Application.Customers.GetCustomers;

public sealed class GetCustomersUseCase : IGetCustomersUseCase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IValidator<GetCustomersQuery> _validator;

    public GetCustomersUseCase(ICustomerRepository customerRepository, IValidator<GetCustomersQuery> validator)
    {
        _customerRepository = customerRepository;
        _validator = validator;
    }

    public async Task<PagedResult<CustomerResponse>> ExecuteAsync(GetCustomersQuery query, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(query, cancellationToken);

        var (items, totalCount) = await _customerRepository.GetPagedAsync(query.Search, query.Page, query.PageSize, cancellationToken);

        return new PagedResult<CustomerResponse>(items.Select(CustomerResponse.From).ToList(), totalCount, query.Page, query.PageSize);
    }
}
