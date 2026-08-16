using Waymark;

namespace Waymark.Tests;

public class ProviderIdTests
{
    [Theory]
    [InlineData("marten-postgresql", ProviderId.MartenPostgresql)]
    [InlineData("efcore-sqlserver", ProviderId.EfCoreSqlServer)]
    [InlineData(" MARTEN-POSTGRESQL ", ProviderId.MartenPostgresql)]
    public void Parses_supported_provider_ids(string value, ProviderId expected)
    {
        var parsed = ProviderIds.TryParse(value, out var provider);

        Assert.True(parsed);
        Assert.Equal(expected, provider);
    }

    [Theory]
    [InlineData("")]
    [InlineData("unknown")]
    [InlineData("postgresql")]
    public void Rejects_unknown_provider_ids(string value)
    {
        var parsed = ProviderIds.TryParse(value, out _);

        Assert.False(parsed);
    }
}
