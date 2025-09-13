using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Bepixeplore.Data;

/* This is used if database provider does't define
 * IBepixeploreDbSchemaMigrator implementation.
 */
public class NullBepixeploreDbSchemaMigrator : IBepixeploreDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
