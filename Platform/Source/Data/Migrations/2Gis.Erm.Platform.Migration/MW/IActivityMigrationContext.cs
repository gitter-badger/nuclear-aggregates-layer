using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.CRM;

namespace DoubleGis.Erm.Platform.Migration.MW
{
    /// <summary>
    /// Represents the context to migrate the entities from CRM to ERM.
    /// </summary>
    public interface IActivityMigrationContext : IMigrationContext, ICrmMigrationContext
    {
        /// <summary>
        /// Provides a new identity.
        /// </summary>
        long NewIdentity();
    }
}