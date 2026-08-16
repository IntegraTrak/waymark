namespace Waymark;

public enum ProviderId
{
    MartenPostgresql,
    EfCoreSqlServer,
}

public static class ProviderIds
{
    public const string MartenPostgresqlName = "marten-postgresql";
    public const string EfCoreSqlServerName = "efcore-sqlserver";

    public static bool TryParse(string value, out ProviderId provider)
    {
        switch (value.Trim().ToLowerInvariant())
        {
            case MartenPostgresqlName:
                provider = ProviderId.MartenPostgresql;
                return true;
            case EfCoreSqlServerName:
                provider = ProviderId.EfCoreSqlServer;
                return true;
            default:
                provider = default;
                return false;
        }
    }
}
