using DoubleGis.Erm.Platform.Migration.Base;

using Microsoft.Xrm.Client.Data.Services;

namespace DoubleGis.Erm.Platform.Migration.CRM
{
    public class CrmMigrationContext : ICrmMigrationContext
    {
        public CrmMigrationContext(CrmDataContext dataContext, MigrationOptions options)
        {
            CrmContext = dataContext;
            Options = options;
        }

        public CrmDataContext CrmContext { get; private set; }
        public MigrationOptions Options { get; private set; }
    }
}
