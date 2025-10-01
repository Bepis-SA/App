using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Bepixplore.Data;
using Volo.Abp.DependencyInjection;

namespace Bepixplore.EntityFrameworkCore;

public class EntityFrameworkCoreBepixploreDbSchemaMigrator
    : IBepixploreDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreBepixploreDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the BepixploreDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<BepixploreDbContext>()
            .Database
            .MigrateAsync();
    }
}
