using Microsoft.Xrm.Client.Data.Services;

namespace DoubleGis.Erm.Platform.Migration.CRM
{
    public class CrmMigrationContext : ICrmMigrationContext
    {
        public CrmMigrationContext(CrmDataContext dataContext)
        {
            CrmContext = dataContext;
        }

        public CrmDataContext CrmContext { get; private set; }
    }
}
