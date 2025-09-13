using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Bepixplore.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class BepixploreDbContextFactory : IDesignTimeDbContextFactory<BepixploreDbContext>
{
    public BepixploreDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        BepixploreEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<BepixploreDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new BepixploreDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Bepixplore.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
