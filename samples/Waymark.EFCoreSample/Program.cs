using Microsoft.EntityFrameworkCore;

var options = new DbContextOptionsBuilder<WaymarkDbContext>()
    .UseSqlServer("Server=localhost;Database=WaymarkSample;User Id=sa;Password=Passw0rd!;TrustServerCertificate=True")
    .Options;

await using var context = new WaymarkDbContext(options);
Console.WriteLine($"Waymark EF Core sample model loaded: {context.Model.GetEntityTypes().Single().Name}");

public sealed class WaymarkDbContext(DbContextOptions<WaymarkDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
}

public sealed class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
