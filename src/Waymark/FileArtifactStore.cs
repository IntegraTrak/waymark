using System.Text.Json;

namespace Waymark;

public sealed class FileArtifactStore(string directory) : IArtifactStore
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public async Task SaveAsync(MigrationArtifact artifact, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(directory);
        var safeId = string.Concat(artifact.MigrationId.Select(character =>
            Path.GetInvalidFileNameChars().Contains(character) ? '_' : character));
        var path = Path.Combine(directory, $"{artifact.Order:D8}_{safeId}.sql");
        var metadataPath = Path.ChangeExtension(path, ".waymark.json");

        if (File.Exists(path) || File.Exists(metadataPath))
        {
            throw new MigrationContractException($"Duplicate migration artifact '{artifact.MigrationId}'.");
        }

        await File.WriteAllTextAsync(path, artifact.Payload, cancellationToken);
        var descriptor = ArtifactDescriptor.FromArtifact(artifact);
        await using var metadata = File.Create(metadataPath);
        await JsonSerializer.SerializeAsync(metadata, descriptor, JsonOptions, cancellationToken);
    }

    public async Task<IReadOnlyList<MigrationArtifact>> ReadAsync(
        ProviderId provider,
        DatabaseEngine databaseEngine,
        CancellationToken cancellationToken = default
    )
    {
        IEnumerable<string> files = Directory.Exists(directory)
            ? Directory.EnumerateFiles(directory, "*.sql").OrderBy(path => path, StringComparer.Ordinal)
            : Array.Empty<string>();
        var artifacts = new List<MigrationArtifact>();
        foreach (var path in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var metadataPath = Path.ChangeExtension(path, ".waymark.json");
            ArtifactDescriptor? descriptor = null;
            if (File.Exists(metadataPath))
            {
                await using var metadata = File.OpenRead(metadataPath);
                descriptor = await JsonSerializer.DeserializeAsync<ArtifactDescriptor>(metadata, cancellationToken: cancellationToken);
            }

            var name = Path.GetFileNameWithoutExtension(path);
            var separator = name.IndexOf('_');
            var order = separator > 0 && long.TryParse(name[..separator], out var parsed) ? parsed : artifacts.Count;
            var migrationId = separator > 0 ? name[(separator + 1)..] : name;
            if (descriptor is not null)
            {
                if (descriptor.Provider != provider || descriptor.DatabaseEngine != databaseEngine || descriptor.MigrationId != migrationId)
                {
                    throw new MigrationContractException($"Artifact metadata does not match '{path}'.");
                }

                order = descriptor.Order;
                migrationId = descriptor.MigrationId;
            }

            artifacts.Add(new MigrationArtifact(
                descriptor?.FormatVersion ?? 1,
                provider,
                databaseEngine,
                migrationId,
                order,
                descriptor?.PayloadKind ?? MigrationPayloadKind.Sql,
                await File.ReadAllTextAsync(path, cancellationToken),
                descriptor?.ExecutionRequirements ?? MigrationExecutionRequirements.None,
                new EmptyProviderArtifactMetadata(),
                descriptor?.SourceIdentity,
                descriptor?.ModelIdentity));
        }

        return MigrationArtifactRules.ValidateAndOrder(provider, databaseEngine, artifacts);
    }

    private sealed record ArtifactDescriptor(
        int FormatVersion,
        ProviderId Provider,
        DatabaseEngine DatabaseEngine,
        string MigrationId,
        long Order,
        MigrationPayloadKind PayloadKind,
        MigrationExecutionRequirements ExecutionRequirements,
        string? SourceIdentity,
        string? ModelIdentity
    )
    {
        public static ArtifactDescriptor FromArtifact(MigrationArtifact artifact) => new(
            artifact.FormatVersion,
            artifact.Provider,
            artifact.DatabaseEngine,
            artifact.MigrationId,
            artifact.Order,
            artifact.PayloadKind,
            artifact.ExecutionRequirements,
            artifact.SourceIdentity,
            artifact.ModelIdentity);
    }
}
