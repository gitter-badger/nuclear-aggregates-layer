using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public class SqlServerConfiguration : DbConfiguration
    {
        public SqlServerConfiguration()
        {
            SetProviderServices(SqlProviderServices.ProviderInvariantName, SqlProviderServices.Instance);
        }
    }
}