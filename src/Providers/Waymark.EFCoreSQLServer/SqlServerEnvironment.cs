using Testcontainers.MsSql;
using Waymark;

namespace Waymark.EFCoreSQLServer;

public sealed class SqlServerEnvironment : IIsolatedDatabaseEnvironment
{
    private readonly MsSqlContainer _container;

    public SqlServerEnvironment(string image = "mcr.microsoft.com/mssql/server:2022-latest")
    {
        _container = new MsSqlBuilder(image)
            .WithPassword("Waymark!12345")
            .Build();
    }

    public string ConnectionString => _container.GetConnectionString();
    public bool IsReady { get; private set; }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await _container.StartAsync(cancellationToken);
        IsReady = true;
    }

    public ValueTask DisposeAsync() => _container.DisposeAsync();
}
