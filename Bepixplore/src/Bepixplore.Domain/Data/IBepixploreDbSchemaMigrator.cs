using System.Threading.Tasks;

namespace Bepixplore.Data;

public interface IBepixploreDbSchemaMigrator
{
    Task MigrateAsync();
}
