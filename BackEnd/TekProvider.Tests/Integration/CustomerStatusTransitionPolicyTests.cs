using TekProvider.Domain.Enums;
using TekProvider.Infrastructure.Policies;

namespace TekProvider.Tests.Integration;

[Collection("Postgres")]
public class CustomerStatusTransitionPolicyTests
{
    private readonly PostgresFixture _fixture;

    public CustomerStatusTransitionPolicyTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [InlineData(CustomerStatus.Active, CustomerStatus.Inactive, true)]
    [InlineData(CustomerStatus.Active, CustomerStatus.Suspended, true)]
    [InlineData(CustomerStatus.Inactive, CustomerStatus.Active, true)]
    [InlineData(CustomerStatus.Suspended, CustomerStatus.Active, true)]
    [InlineData(CustomerStatus.Inactive, CustomerStatus.Suspended, false)]
    [InlineData(CustomerStatus.Suspended, CustomerStatus.Inactive, false)]
    [InlineData(CustomerStatus.Active, CustomerStatus.Active, false)]
    public async Task IsTransitionValidAsync_MatchesSeededMatrix(CustomerStatus from, CustomerStatus to, bool expected)
    {
        await using var dbContext = _fixture.CreateDbContext();
        var policy = new CustomerStatusTransitionPolicy(dbContext);

        var result = await policy.IsTransitionValidAsync(from, to);

        Assert.Equal(expected, result);
    }
}
