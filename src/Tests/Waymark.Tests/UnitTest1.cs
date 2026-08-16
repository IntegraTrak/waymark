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

    [Fact]
    public void Orders_artifacts_deterministically_and_rejects_duplicates()
    {
        var artifacts = new[]
        {
            new MigrationArtifact(1, ProviderId.MartenPostgresql, DatabaseEngine.PostgreSql, "second", 2, MigrationPayloadKind.Sql, "SELECT 2;", MigrationExecutionRequirements.Idempotent, new EmptyProviderArtifactMetadata()),
            new MigrationArtifact(1, ProviderId.MartenPostgresql, DatabaseEngine.PostgreSql, "first", 1, MigrationPayloadKind.Sql, "SELECT 1;", MigrationExecutionRequirements.Idempotent, new EmptyProviderArtifactMetadata()),
        };

        var ordered = MigrationArtifactRules.ValidateAndOrder(ProviderId.MartenPostgresql, DatabaseEngine.PostgreSql, artifacts);

        Assert.Equal(["first", "second"], ordered.Select(artifact => artifact.MigrationId));
        Assert.Throws<MigrationContractException>(() => MigrationArtifactRules.ValidateAndOrder(ProviderId.MartenPostgresql, DatabaseEngine.PostgreSql, [artifacts[0], artifacts[0]]));
    }

    [Fact]
    public void Rejects_artifacts_for_another_provider_or_engine()
    {
        var artifact = new MigrationArtifact(1, ProviderId.EfCoreSqlServer, DatabaseEngine.SqlServer, "migration", 1, MigrationPayloadKind.Sql, "SELECT 1;", MigrationExecutionRequirements.None, new EmptyProviderArtifactMetadata());

        Assert.Throws<MigrationContractException>(() => MigrationArtifactRules.ValidateAndOrder(ProviderId.MartenPostgresql, DatabaseEngine.PostgreSql, [artifact]));
    }

    [Fact]
    public void Composes_ordered_sql_without_database_access()
    {
        var composer = new SqlScriptComposer();
        var artifacts = new[]
        {
            new MigrationArtifact(1, ProviderId.MartenPostgresql, DatabaseEngine.PostgreSql, "second", 2, MigrationPayloadKind.Sql, "SELECT 2;", MigrationExecutionRequirements.None, new EmptyProviderArtifactMetadata()),
            new MigrationArtifact(1, ProviderId.MartenPostgresql, DatabaseEngine.PostgreSql, "first", 1, MigrationPayloadKind.Sql, "SELECT 1;", MigrationExecutionRequirements.None, new EmptyProviderArtifactMetadata()),
        };

        var script = composer.Compose(ProviderId.MartenPostgresql, DatabaseEngine.PostgreSql, artifacts);

        Assert.True(script.IndexOf("SELECT 1;", StringComparison.Ordinal) < script.IndexOf("SELECT 2;", StringComparison.Ordinal));
    }

    [Fact]
    public async Task File_artifact_store_round_trips_common_metadata()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"waymark-artifacts-{Guid.NewGuid():N}");
        try
        {
            var artifact = new MigrationArtifact(1, ProviderId.EfCoreSqlServer, DatabaseEngine.SqlServer, "initial", 1, MigrationPayloadKind.Sql, "SELECT 1;", MigrationExecutionRequirements.Transaction, new EmptyProviderArtifactMetadata(), "sample.csproj", "model-v1");
            var store = new FileArtifactStore(directory);

            await store.SaveAsync(artifact);
            var result = await store.ReadAsync(ProviderId.EfCoreSqlServer, DatabaseEngine.SqlServer);

            var roundTripped = Assert.Single(result);
            Assert.Equal(artifact.MigrationId, roundTripped.MigrationId);
            Assert.Equal(artifact.ExecutionRequirements, roundTripped.ExecutionRequirements);
            Assert.Equal(artifact.SourceIdentity, roundTripped.SourceIdentity);
            Assert.Equal(artifact.ModelIdentity, roundTripped.ModelIdentity);
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }
}
