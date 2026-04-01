using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Persistence;

public class DbContextFactory : IDesignTimeDbContextFactory<GarneauTemplateDbContext>
{
    public GarneauTemplateDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GarneauTemplateDbContext>();

        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=GarneauTemplate;User Id=sa;Password=Qwerty123!;TrustServerCertificate=True;");

        return new GarneauTemplateDbContext(optionsBuilder.Options);
    }
}