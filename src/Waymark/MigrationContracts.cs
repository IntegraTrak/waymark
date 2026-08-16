namespace Waymark;

public enum DatabaseEngine
{
    PostgreSql,
    SqlServer,
}

public enum MigrationPayloadKind
{
    Sql,
    NativeCode,
}

[Flags]
public enum MigrationExecutionRequirements
{
    None = 0,
    Transaction = 1,
    Idempotent = 2,
    NonTransactional = 4,
}

public interface IProviderMigrationInput;

public sealed record MigrationRequest(
    ProviderId Provider,
    string ProjectPath,
    string? StartupProjectPath = null,
    string? DatabaseName = null,
    string? SchemaName = null,
    IProviderMigrationInput? ProviderInput = null,
    string? ArtifactDirectory = null
);

public sealed record ProviderCapabilities(
    bool SupportsMigrationGeneration,
    bool SupportsScriptComposition,
    bool SupportsTargetExecution,
    MigrationPayloadKind[] PayloadKinds,
    MigrationExecutionRequirements ExecutionRequirements
);

public abstract record ProviderArtifactMetadata;

public sealed record EmptyProviderArtifactMetadata : ProviderArtifactMetadata;

public sealed record NativeMigrationResult(
    string MigrationId,
    long Order,
    MigrationPayloadKind PayloadKind,
    string Payload,
    MigrationExecutionRequirements ExecutionRequirements,
    ProviderArtifactMetadata Metadata,
    string? SourceIdentity = null,
    string? ModelIdentity = null
);

public sealed record MigrationArtifact(
    int FormatVersion,
    ProviderId Provider,
    DatabaseEngine DatabaseEngine,
    string MigrationId,
    long Order,
    MigrationPayloadKind PayloadKind,
    string Payload,
    MigrationExecutionRequirements ExecutionRequirements,
    ProviderArtifactMetadata Metadata,
    string? SourceIdentity = null,
    string? ModelIdentity = null
)
{
    public static MigrationArtifact FromResult(
        ProviderId provider,
        DatabaseEngine databaseEngine,
        NativeMigrationResult result
    ) => new(
        1,
        provider,
        databaseEngine,
        result.MigrationId,
        result.Order,
        result.PayloadKind,
        result.Payload,
        result.ExecutionRequirements,
        result.Metadata,
        result.SourceIdentity,
        result.ModelIdentity
    );
}

public interface IIsolatedDatabaseEnvironment : IAsyncDisposable
{
    string ConnectionString { get; }
    bool IsReady { get; }
    Task StartAsync(CancellationToken cancellationToken = default);
}

public interface IProviderAdapter
{
    ProviderId Provider { get; }
    DatabaseEngine DatabaseEngine { get; }
    ProviderCapabilities Capabilities { get; }
    Task<IIsolatedDatabaseEnvironment> CreateEnvironmentAsync(
        MigrationRequest request,
        CancellationToken cancellationToken = default
    );
    Task<NativeMigrationResult> GenerateMigrationAsync(
        MigrationRequest request,
        IIsolatedDatabaseEnvironment environment,
        CancellationToken cancellationToken = default
    );
}

public interface IArtifactStore
{
    Task SaveAsync(MigrationArtifact artifact, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MigrationArtifact>> ReadAsync(
        ProviderId provider,
        DatabaseEngine databaseEngine,
        CancellationToken cancellationToken = default
    );
}

public interface IScriptComposer
{
    string Compose(
        ProviderId provider,
        DatabaseEngine databaseEngine,
        IReadOnlyCollection<MigrationArtifact> artifacts
    );
}

public sealed class MigrationContractException : InvalidOperationException
{
    public MigrationContractException(string message)
        : base(message) { }
}

public static class MigrationArtifactRules
{
    public static IReadOnlyList<MigrationArtifact> ValidateAndOrder(
        ProviderId provider,
        DatabaseEngine databaseEngine,
        IEnumerable<MigrationArtifact> artifacts
    )
    {
        var ordered = artifacts.OrderBy(artifact => artifact.Order).ThenBy(artifact => artifact.MigrationId).ToArray();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (var artifact in ordered)
        {
            if (artifact.Provider != provider || artifact.DatabaseEngine != databaseEngine)
            {
                throw new MigrationContractException(
                    $"Artifact '{artifact.MigrationId}' does not belong to {provider}/{databaseEngine}."
                );
            }

            if (!seen.Add(artifact.MigrationId))
            {
                throw new MigrationContractException(
                    $"Duplicate migration artifact '{artifact.MigrationId}'."
                );
            }
        }

        return ordered;
    }
}
