using Waymark;
using Waymark.EFCoreSQLServer;
using Waymark.MartenPostgres;

namespace Waymark.Tests;

public sealed class ProviderGenerationTests
{
    [Fact(Timeout = 240_000)]
    public async Task Marten_adapter_generates_a_provider_payload_from_the_sample()
    {
        var root = FindRepositoryRoot();
        var project = Path.Combine(root, "samples", "Waymark.MartenSample", "Waymark.MartenSample.csproj");
        var adapter = new MartenPostgresAdapter();
        var request = new MigrationRequest(
            ProviderId.MartenPostgresql,
            project,
            DatabaseName: "waymark",
            ProviderInput: new MartenMigrationInput("marten-test"));

        await using var environment = await adapter.CreateEnvironmentAsync(request);
        await environment.StartAsync();
        var result = await adapter.GenerateMigrationAsync(request, environment);

        Assert.Equal(MigrationPayloadKind.Sql, result.PayloadKind);
        Assert.NotEmpty(result.Payload);
    }

    [Fact(Timeout = 240_000)]
    public async Task Ef_core_adapter_generates_a_provider_payload_from_the_sample()
    {
        var root = FindRepositoryRoot();
        var project = Path.Combine(root, "samples", "Waymark.EFCoreSample", "Waymark.EFCoreSample.csproj");
        var adapter = new EfCoreSqlServerAdapter();
        var request = new MigrationRequest(
            ProviderId.EfCoreSqlServer,
            project,
            ProviderInput: new EfCoreMigrationInput("WaymarkDbContext"));

        await using var environment = await adapter.CreateEnvironmentAsync(request);
        await environment.StartAsync();
        var result = await adapter.GenerateMigrationAsync(request, environment);

        Assert.Equal(MigrationPayloadKind.Sql, result.PayloadKind);
        Assert.NotEmpty(result.Payload);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Waymark.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the Waymark solution root.");
    }
}
