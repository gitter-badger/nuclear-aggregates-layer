using DoubleGis.Erm.Platform.Migration.Base;

using Microsoft.Xrm.Client.Data.Services;

namespace DoubleGis.Erm.Platform.Migration.CRM
{
    public interface ICrmMigrationContext : IMigrationContextBase
    {
        CrmDataContext CrmContext { get; }
    }
}
