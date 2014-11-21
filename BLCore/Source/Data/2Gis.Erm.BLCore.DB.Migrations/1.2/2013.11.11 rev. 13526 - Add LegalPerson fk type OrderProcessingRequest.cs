using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._2
{
    [Migration(13526, "Добавляем в таблицу OrderProcessingRequests fk на поле LegalPersonId")]
    public class Migration13526 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const string LegalPersonIdColumnName = "LegalPersonId";

            var table = context.Database.GetTable(ErmTableNames.OrderProcessingRequests);

            if (table == null)
            {
                return;
            }

            if (!table.Columns.Contains(LegalPersonIdColumnName))
            {
                return;
            }

            CreateFk(table, "FK_OrderProcessingRequests_LegalPersons", LegalPersonIdColumnName, ErmTableNames.LegalPersons);
        }

        private static void CreateFk(Table table, string fkName, string fieldName, SchemaQualifiedObjectName ermTableName)
        {
            var foreignKey = new ForeignKey(table, fkName);
            foreignKey.Columns.Add(new ForeignKeyColumn(foreignKey, fieldName, "Id"));
            foreignKey.ReferencedTable = ermTableName.Name;
            foreignKey.ReferencedTableSchema = ermTableName.Schema;
            foreignKey.Create();
        }
    }
}
