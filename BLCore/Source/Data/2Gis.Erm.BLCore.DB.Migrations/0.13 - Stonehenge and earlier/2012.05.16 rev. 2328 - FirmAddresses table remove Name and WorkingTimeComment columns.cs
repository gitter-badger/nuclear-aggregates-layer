using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(2328, "Удаляем неиспользуемые колонки из FirmAddresses")]
    public class Migration2328 : TransactedMigration
    {
        private readonly SchemaQualifiedObjectName _firmAddresses = new SchemaQualifiedObjectName(ErmSchemas.BusinessDirectory, "FirmAddresses");

        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables[_firmAddresses.Name, _firmAddresses.Schema];
            table.Columns["Name"].Drop();
            table.Columns["WorkingTimeComment"].Drop();
            table.Alter();
        }
    }
}