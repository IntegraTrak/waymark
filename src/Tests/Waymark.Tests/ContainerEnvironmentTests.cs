using Waymark.EFCoreSQLServer;
using Waymark.MartenPostgres;

namespace Waymark.Tests;

public sealed class ContainerEnvironmentTests
{
    [Fact(Timeout = 180_000)]
    public async Task PostgreSql_environment_starts_and_disposes()
    {
        await using var environment = new PostgreSqlEnvironment();

        await environment.StartAsync();

        Assert.True(environment.IsReady);
        Assert.Contains("Host=", environment.ConnectionString, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(Timeout = 180_000)]
    public async Task SqlServer_environment_starts_and_disposes()
    {
        await using var environment = new SqlServerEnvironment();

        await environment.StartAsync();

        Assert.True(environment.IsReady);
        Assert.Contains("Server=", environment.ConnectionString, StringComparison.OrdinalIgnoreCase);
    }
}
