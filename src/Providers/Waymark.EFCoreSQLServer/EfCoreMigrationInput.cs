using Waymark;

namespace Waymark.EFCoreSQLServer;

public sealed record EfCoreMigrationInput(string? ContextName = null) : IProviderMigrationInput;
