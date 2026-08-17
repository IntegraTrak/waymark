namespace Waymark;

public sealed class ProviderRegistry(IEnumerable<IMigrationProvider> adapters)
{
    private readonly IReadOnlyDictionary<ProviderId, IMigrationProvider> _adapters =
        adapters.ToDictionary(adapter => adapter.Provider);

    public IMigrationProvider Resolve(ProviderId provider) =>
        _adapters.TryGetValue(provider, out var adapter)
            ? adapter
            : throw new MigrationContractException($"Provider '{provider}' is not registered.");
}

public sealed class MigrationOrchestrator(
    ProviderRegistry providers,
    IArtifactStore artifacts
)
{
    public async Task<MigrationArtifact> GenerateAsync(
        MigrationRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var adapter = providers.Resolve(request.Provider);
        var databaseEngine = adapter.DatabaseEngine;

        var existing = await artifacts.ReadAsync(adapter.Provider, databaseEngine, cancellationToken);
        await using var environment = await adapter.CreateEnvironmentAsync(request, cancellationToken);
        await environment.StartAsync(cancellationToken);
        var result = await adapter.GenerateMigrationAsync(request, environment, cancellationToken);
        var artifact = MigrationArtifact.FromResult(adapter.Provider, databaseEngine, result) with
        {
            Order = existing.Count == 0 ? result.Order : existing.Max(existingArtifact => existingArtifact.Order) + 1,
        };
        await artifacts.SaveAsync(artifact, cancellationToken);
        return artifact;
    }
}
