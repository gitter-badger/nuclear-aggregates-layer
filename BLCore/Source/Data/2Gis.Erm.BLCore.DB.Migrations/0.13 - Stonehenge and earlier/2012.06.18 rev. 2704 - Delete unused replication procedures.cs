using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(2704, "Удаление неиспользуемых хранимых процедур")]
    public class Migration2704 : TransactedMigration
    {
        private readonly SchemaQualifiedObjectName[] _storedProceduresToRemove = new[]
		{
			new SchemaQualifiedObjectName(ErmSchemas.BusinessDirectory, "ReplicateCategories"),
			new SchemaQualifiedObjectName(ErmSchemas.BusinessDirectory, "ReplicateFirmAddresses"),
			new SchemaQualifiedObjectName(ErmSchemas.BusinessDirectory, "ReplicateFirms")
		};

        protected override void ApplyOverride(IMigrationContext context)
        {
            foreach (var storedProcedureToRemove in _storedProceduresToRemove)
            {
                if (!context.Database.StoredProcedures.Contains(storedProcedureToRemove.Name, storedProcedureToRemove.Schema))
                    continue;

                var storedProcedure = context.Database.StoredProcedures[storedProcedureToRemove.Name, storedProcedureToRemove.Schema];
                storedProcedure.Drop();
            }
        }
    }
}