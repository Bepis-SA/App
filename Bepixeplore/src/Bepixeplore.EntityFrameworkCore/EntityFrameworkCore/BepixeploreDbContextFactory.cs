using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Bepixeplore.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class BepixeploreDbContextFactory : IDesignTimeDbContextFactory<BepixeploreDbContext>
{
    public BepixeploreDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        BepixeploreEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<BepixeploreDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new BepixeploreDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Bepixeplore.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
