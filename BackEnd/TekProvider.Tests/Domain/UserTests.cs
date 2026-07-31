using TekProvider.Domain.Entities;

namespace TekProvider.Tests.Domain;

public class UserTests
{
    [Fact]
    public void Create_SetsExpectedDefaults()
    {
        var user = User.Create("erick", "hashed-password");

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("erick", user.Username);
        Assert.Equal("hashed-password", user.PasswordHash);
    }
}
