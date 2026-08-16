using System.Diagnostics;
using Waymark;

namespace Waymark.EFCoreSQLServer;

public sealed class EfCoreSqlServerAdapter : IProviderAdapter
{
    public ProviderId Provider => ProviderId.EfCoreSqlServer;
    public DatabaseEngine DatabaseEngine => DatabaseEngine.SqlServer;
    public ProviderCapabilities Capabilities { get; } = new(
        SupportsMigrationGeneration: true,
        SupportsScriptComposition: true,
        SupportsTargetExecution: false,
        [MigrationPayloadKind.Sql],
        MigrationExecutionRequirements.Transaction
    );

    public Task<IIsolatedDatabaseEnvironment> CreateEnvironmentAsync(
        MigrationRequest request,
        CancellationToken cancellationToken = default
    ) => Task.FromResult<IIsolatedDatabaseEnvironment>(new SqlServerEnvironment());

    public async Task<NativeMigrationResult> GenerateMigrationAsync(
        MigrationRequest request,
        IIsolatedDatabaseEnvironment environment,
        CancellationToken cancellationToken = default
    )
    {
        if (!environment.IsReady)
        {
            throw new MigrationContractException("The isolated SQL Server environment is not ready.");
        }

        var outputPath = Path.Combine(Path.GetTempPath(), $"waymark-{Guid.NewGuid():N}.sql");
        try
        {
            var arguments = new List<string>
            {
                "ef",
                "migrations",
                "script",
                "--project",
                request.ProjectPath,
                "--output",
                outputPath,
                "--no-build",
            };

            if (request.ProviderInput is EfCoreMigrationInput { ContextName: { Length: > 0 } contextName })
            {
                arguments.Add("--context");
                arguments.Add(contextName);
            }

            var result = await RunAsync("dotnet", arguments, Path.GetDirectoryName(request.ProjectPath), cancellationToken);
            if (result.ExitCode != 0)
            {
                throw new MigrationContractException($"EF Core migration script generation failed: {result.StandardError}");
            }

            var payload = await File.ReadAllTextAsync(outputPath, cancellationToken);
            return new NativeMigrationResult(
                request.DatabaseName is { Length: > 0 } name ? name : "efcore-script",
                1,
                MigrationPayloadKind.Sql,
                payload,
                MigrationExecutionRequirements.Transaction,
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

    private static async Task<ProcessResult> RunAsync(
        string fileName,
        IReadOnlyList<string> arguments,
        string? workingDirectory,
        CancellationToken cancellationToken
    )
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };
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
