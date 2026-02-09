using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Bepixplore.Data;

/* This is used if database provider does't define
 * IBepixploreDbSchemaMigrator implementation.
 */
public class NullBepixploreDbSchemaMigrator : IBepixploreDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
