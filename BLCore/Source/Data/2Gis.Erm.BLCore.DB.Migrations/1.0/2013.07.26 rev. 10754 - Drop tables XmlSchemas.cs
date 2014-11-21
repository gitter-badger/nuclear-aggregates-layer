using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(10754, "Удаляем таблицу XmlSchemas")]
    public class Migration10754 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var tableMessageTypes = context.Database.GetTable(ErmTableNames.MessageTypes);
            var foreignKey = tableMessageTypes.ForeignKeys["FK_MessageTypes_XmlSchemas"];
            if (foreignKey != null)
            {
                foreignKey.Drop();
            }

            var column = tableMessageTypes.Columns["XmlSchemaId"];
            if (column != null)
            {
                column.Drop();
            }

            var tableXmlSchemas = context.Database.GetTable(ErmTableNames.XmlSchemas);
            if (tableXmlSchemas != null)
            {
                tableXmlSchemas.Drop();
            }
        }
    }
}