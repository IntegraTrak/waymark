using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public sealed class WaymarkDbContextFactory : IDesignTimeDbContextFactory<WaymarkDbContext>
{
    public WaymarkDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<WaymarkDbContext>()
            .UseSqlServer("Server=localhost;Database=WaymarkSample;User Id=sa;Password=Passw0rd!;TrustServerCertificate=True")
            .Options;

        return new WaymarkDbContext(options);
    }
}
