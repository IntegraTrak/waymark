using Marten;

public static class ModelStates
{
    public static StoreOptions CreateEarlier()
    {
        var options = new StoreOptions();
        options.Connection("Host=localhost;Database=waymark_sample;Username=postgres;Password=postgres");
        options.Schema.For<CustomerV1>();
        return options;
    }

    public static StoreOptions CreateLater()
    {
        var options = new StoreOptions();
        options.Connection("Host=localhost;Database=waymark_sample;Username=postgres;Password=postgres");
        options.Schema.For<CustomerV2>();
        return options;
    }
}

public sealed class CustomerV1
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class CustomerV2
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
