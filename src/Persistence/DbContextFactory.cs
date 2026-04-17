using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Persistence;

public class DbContextFactory : IDesignTimeDbContextFactory<GarneauTemplateDbContext>
{
    public GarneauTemplateDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GarneauTemplateDbContext>();

        //optionsBuilder.UseSqlServer("Server=localhost;Database=GarneauTemplate;User Id=sa;Password=Qwerty123!;TrustServerCertificate=True;");
        optionsBuilder.UseSqlServer("Server=localhost;Database=GarneauTemplate;Trusted_Connection=True;TrustServerCertificate=True;");
        return new GarneauTemplateDbContext(optionsBuilder.Options);
    }
}