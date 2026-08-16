using Spectre.Console;
using Spectre.Console.Cli;
using Waymark;

var app = new CommandApp<RootCommand>();
app.Configure(configuration =>
{
    configuration.SetApplicationName("waymark");
    configuration.SetApplicationVersion("0.1.0");
});

return await app.RunAsync(args);

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
