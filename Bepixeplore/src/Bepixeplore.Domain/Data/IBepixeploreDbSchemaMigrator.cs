using System.Threading.Tasks;

namespace Bepixeplore.Data;

public interface IBepixeploreDbSchemaMigrator
{
    Task MigrateAsync();
}
