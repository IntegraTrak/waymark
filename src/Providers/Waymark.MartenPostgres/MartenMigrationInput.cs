using Waymark;

namespace Waymark.MartenPostgres;

public sealed record MartenMigrationInput(
    string? MigrationName = null,
    string CommandName = "db-patch",
    string ModelState = "later"
) : IProviderMigrationInput;
