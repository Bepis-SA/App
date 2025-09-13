using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Bepixeplore.Data;
using Volo.Abp.DependencyInjection;

namespace Bepixeplore.EntityFrameworkCore;

public class EntityFrameworkCoreBepixeploreDbSchemaMigrator
    : IBepixeploreDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreBepixeploreDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the BepixeploreDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<BepixeploreDbContext>()
            .Database
            .MigrateAsync();
    }
}
