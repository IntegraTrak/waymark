using JasperFx;
using Marten;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ApplyJasperFxExtensions();
builder.Services.AddMarten(_ =>
{
    var options = new StoreOptions();
    options.Connection(builder.Configuration["WAYMARK_CONNECTION_STRING"] ?? "Host=localhost;Database=waymark_sample;Username=postgres;Password=postgres");
    var earlier = string.Equals(builder.Configuration["WAYMARK_MODEL_STATE"], "earlier", StringComparison.OrdinalIgnoreCase);
    if (earlier)
    {
        options.Schema.For<CustomerV1>();
    }
    else
    {
        options.Schema.For<CustomerV2>();
    }

    return options;
});

var app = builder.Build();
app.MapGet("/", () => "Waymark Marten sample");

return await app.RunJasperFxCommands(args);
