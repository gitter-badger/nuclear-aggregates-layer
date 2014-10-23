using DoubleGis.Erm.Platform.Migration.MW;

namespace DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration
{
    /// <summary>
    /// Extends the context to resolve CRM types.
    /// </summary>
    internal interface IActivityMigrationContextExtended : IActivityMigrationContext
    {
        /// <summary>
        /// Converts CRM types to .NET ones and casts it to the specified type.
        /// </summary>
        T Parse<T>(object value);
    }
}