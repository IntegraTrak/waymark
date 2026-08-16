using System.Diagnostics;
using Waymark;

namespace Waymark.MartenPostgres;

public sealed class MartenPostgresAdapter : IProviderAdapter
{
    public ProviderId Provider => ProviderId.MartenPostgresql;
    public DatabaseEngine DatabaseEngine => DatabaseEngine.PostgreSql;
    public ProviderCapabilities Capabilities { get; } = new(
        SupportsMigrationGeneration: true,
        SupportsScriptComposition: true,
        SupportsTargetExecution: false,
        [MigrationPayloadKind.Sql],
        MigrationExecutionRequirements.Transaction | MigrationExecutionRequirements.Idempotent
    );

    public Task<IIsolatedDatabaseEnvironment> CreateEnvironmentAsync(
        MigrationRequest request,
        CancellationToken cancellationToken = default
    ) => Task.FromResult<IIsolatedDatabaseEnvironment>(new PostgreSqlEnvironment());

    public async Task<NativeMigrationResult> GenerateMigrationAsync(
        MigrationRequest request,
        IIsolatedDatabaseEnvironment environment,
        CancellationToken cancellationToken = default
    )
    {
        if (!environment.IsReady)
        {
            throw new MigrationContractException("The isolated PostgreSQL environment is not ready.");
        }

        var input = request.ProviderInput as MartenMigrationInput ?? new MartenMigrationInput();
        await ApplyExistingArtifactsAsync(request, environment.ConnectionString, cancellationToken);
        var outputPath = Path.Combine(Path.GetTempPath(), $"waymark-{Guid.NewGuid():N}.sql");
        var arguments = new List<string>
        {
            "run",
            "--project",
            request.ProjectPath,
            "--",
            input.CommandName,
            outputPath,
        };

        var result = await RunAsync(
            arguments,
            Path.GetDirectoryName(request.ProjectPath),
            environment.ConnectionString,
            input.ModelState,
            cancellationToken
        );
        if (result.ExitCode != 0)
        {
            throw new MigrationContractException($"Marten migration generation failed: {result.StandardError}");
        }

        try
        {
            var payload = await File.ReadAllTextAsync(outputPath, cancellationToken);
            return new NativeMigrationResult(
                input.MigrationName ?? Path.GetFileNameWithoutExtension(outputPath),
                1,
                MigrationPayloadKind.Sql,
                payload,
                MigrationExecutionRequirements.Transaction | MigrationExecutionRequirements.Idempotent,
                new EmptyProviderArtifactMetadata(),
                request.ProjectPath
            );
        }
        finally
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
        }
    }

    private static async Task ApplyExistingArtifactsAsync(
        MigrationRequest request,
        string connectionString,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(request.ArtifactDirectory) || !Directory.Exists(request.ArtifactDirectory))
        {
            return;
        }

        var store = new FileArtifactStore(request.ArtifactDirectory);
        var artifacts = await store.ReadAsync(ProviderId.MartenPostgresql, DatabaseEngine.PostgreSql, cancellationToken);
        if (artifacts.Count == 0)
        {
            return;
        }

        await using var dataSource = Npgsql.NpgsqlDataSource.Create(connectionString);
        await using var command = dataSource.CreateCommand(string.Join(Environment.NewLine, artifacts.Select(artifact => artifact.Payload)));
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task<ProcessResult> RunAsync(
        IReadOnlyList<string> arguments,
        string? workingDirectory,
        string connectionString,
        string modelState,
        CancellationToken cancellationToken
    )
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };
        process.StartInfo.Environment["WAYMARK_CONNECTION_STRING"] = connectionString;
        process.StartInfo.Environment["WAYMARK_MODEL_STATE"] = modelState;
        foreach (var argument in arguments)
        {
            process.StartInfo.ArgumentList.Add(argument);
        }

        process.Start();
        var output = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var error = process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);
        return new ProcessResult(process.ExitCode, await output, await error);
    }

    private sealed record ProcessResult(int ExitCode, string StandardOutput, string StandardError);
}
