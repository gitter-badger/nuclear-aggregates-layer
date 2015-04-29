using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace NuClear.Storage.EntityFramework
{
    public class SqlServerConfiguration : DbConfiguration
    {
        public SqlServerConfiguration()
        {
            SetProviderServices(SqlProviderServices.ProviderInvariantName, SqlProviderServices.Instance);
        }
    }
}