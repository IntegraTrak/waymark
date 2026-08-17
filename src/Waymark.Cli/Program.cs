using Spectre.Console;
using Spectre.Console.Cli;
using Waymark;

var app = new CommandApp<RootCommand>();
app.Configure(configuration =>
{
    configuration.SetApplicationName("waymark");
    configuration.SetApplicationVersion("0.1.0");
    configuration.AddBranch("migrations", migrations =>
    {
        migrations.AddCommand<AddCommand>("add");
        migrations.AddCommand<ScriptCommand>("script");
    });
});

var commandArgs = args;
var migrationsIndex = Array.IndexOf(args, "migrations");
var providerIndex = Array.IndexOf(args, "--provider");
if (migrationsIndex >= 0 && providerIndex >= 0 && providerIndex < migrationsIndex && providerIndex + 1 < args.Length)
{
    var normalized = args.ToList();
    var provider = normalized[providerIndex + 1];
    normalized.RemoveAt(providerIndex + 1);
    normalized.RemoveAt(providerIndex);
    normalized.Add("--provider");
    normalized.Add(provider);
    commandArgs = normalized.ToArray();
}

return await app.RunAsync(commandArgs);

sealed class RootSettings : CommandSettings
{
    [CommandOption("--provider <PROVIDER>")]
    public string? Provider { get; init; }
}

sealed class RootCommand : AsyncCommand<RootSettings>
{
    public override Task<int> ExecuteAsync(CommandContext context, RootSettings settings, CancellationToken cancellationToken)
    {
        if (settings.Provider is null)
        {
            AnsiConsole.MarkupLine("Waymark CLI is ready. Supply [yellow]--provider[/] for provider operations.");
            return Task.FromResult(0);
        }

        if (!ProviderIds.TryParse(settings.Provider, out var provider))
        {
            AnsiConsole.MarkupLineInterpolated($"[red]Unknown provider:[/] {settings.Provider}");
            AnsiConsole.MarkupLine($"Supported providers: {ProviderIds.MartenPostgresqlName}, {ProviderIds.EfCoreSqlServerName}");
            return Task.FromResult(1);
        }

        AnsiConsole.MarkupLineInterpolated($"Provider selected: [green]{provider}[/]");
        return Task.FromResult(0);
    }
}

sealed class AddSettings : CommandSettings
{
    [CommandOption("--provider <PROVIDER>")]
    public string? Provider { get; init; }

    [CommandOption("--project <PATH>")]
    public string? ProjectPath { get; init; }

    [CommandOption("--artifacts <PATH>")]
    public string? ArtifactsPath { get; init; }

    [CommandOption("--database <NAME>")]
    public string? DatabaseName { get; init; }

    [CommandOption("--schema <NAME>")]
    public string? SchemaName { get; init; }

    [CommandOption("--context <NAME>")]
    public string? ContextName { get; init; }

    [CommandOption("--name <NAME>")]
    public string? MigrationName { get; init; }

    [CommandOption("--model-state <STATE>")]
    public string ModelState { get; init; } = "later";
}

sealed class AddCommand : AsyncCommand<AddSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, AddSettings settings, CancellationToken cancellationToken)
    {
        if (settings.Provider is null || !ProviderIds.TryParse(settings.Provider, out var provider))
        {
            AnsiConsole.MarkupLine("[red]A valid --provider is required.[/]");
            return 1;
        }

        if (string.IsNullOrWhiteSpace(settings.ProjectPath) || !File.Exists(settings.ProjectPath))
        {
            AnsiConsole.MarkupLine("[red]An existing --project file is required.[/]");
            return 1;
        }

        var adapter = provider == ProviderId.MartenPostgresql
            ? (IMigrationProvider)new Waymark.MartenPostgres.MartenPostgresAdapter()
            : new Waymark.EFCoreSQLServer.EfCoreSqlServerAdapter();
        var request = new MigrationRequest(
            provider,
            Path.GetFullPath(settings.ProjectPath),
            DatabaseName: settings.DatabaseName,
            SchemaName: settings.SchemaName,
            ProviderInput: provider == ProviderId.MartenPostgresql
                ? new Waymark.MartenPostgres.MartenMigrationInput(settings.MigrationName, ModelState: settings.ModelState)
                : new Waymark.EFCoreSQLServer.EfCoreMigrationInput(settings.ContextName),
            ArtifactDirectory: settings.ArtifactsPath);
        var artifactStore = new FileArtifactStore(settings.ArtifactsPath ?? Path.Combine("waymark", provider.ToString()));
        var orchestrator = new MigrationOrchestrator(new ProviderRegistry([adapter]), artifactStore);
        var artifact = await orchestrator.GenerateAsync(request, cancellationToken);
        AnsiConsole.MarkupLineInterpolated($"Generated migration artifact [green]{artifact.MigrationId}[/].");
        return 0;
    }
}

sealed class ScriptSettings : CommandSettings
{
    [CommandOption("--provider <PROVIDER>")]
    public string? Provider { get; init; }

    [CommandOption("--artifacts <PATH>")]
    public string? ArtifactsPath { get; init; }

    [CommandOption("--output <PATH>")]
    public string? OutputPath { get; init; }
}

sealed class ScriptCommand : AsyncCommand<ScriptSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, ScriptSettings settings, CancellationToken cancellationToken)
    {
        if (settings.Provider is null || !ProviderIds.TryParse(settings.Provider, out var provider))
        {
            AnsiConsole.MarkupLine("[red]A valid --provider is required.[/]");
            return 1;
        }

        if (string.IsNullOrWhiteSpace(settings.ArtifactsPath) || !Directory.Exists(settings.ArtifactsPath))
        {
            AnsiConsole.MarkupLine("[red]An existing --artifacts directory is required.[/]");
            return 1;
        }

        var engine = provider == ProviderId.MartenPostgresql
            ? DatabaseEngine.PostgreSql
            : DatabaseEngine.SqlServer;
        var artifacts = await new FileArtifactStore(settings.ArtifactsPath).ReadAsync(provider, engine, cancellationToken);
        var script = new SqlScriptComposer().Compose(provider, engine, artifacts);
        if (string.IsNullOrWhiteSpace(settings.OutputPath))
        {
            AnsiConsole.WriteLine(script);
            return 0;
        }

        File.WriteAllText(settings.OutputPath, script);
        AnsiConsole.MarkupLineInterpolated($"Wrote migration script to [green]{settings.OutputPath}[/]");
        return 0;
    }
}
