using Testcontainers.PostgreSql;
using Waymark;

namespace Waymark.MartenPostgres;

public sealed class PostgreSqlEnvironment : IIsolatedDatabaseEnvironment
{
    private readonly PostgreSqlContainer _container;

    public PostgreSqlEnvironment(string image = "postgres:16-alpine")
    {
        _container = new PostgreSqlBuilder(image)
            .WithDatabase("waymark")
            .WithUsername("postgres")
            .WithPassword("postgres")
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
