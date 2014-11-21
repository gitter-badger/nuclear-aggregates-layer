using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(22717, "Создание таблицы Billing.ClientLinks", "f.zaharov")]
    public class Migration22717 : TransactedMigration
    {
        const string Id = "Id";
        const string MasterClientId = "MasterClientId";
        const string ChildClientId = "ChildClientId";
        const string CreatedBy = "CreatedBy";
        const string ModifiedBy = "ModifiedBy";
        const string CreatedOn = "CreatedOn";
        const string ModifiedOn = "ModifiedOn";
        const string IsDeleted = "IsDeleted";

        protected override void ApplyOverride(IMigrationContext context)
        {
            if (context.Database.GetTable(ErmTableNames.ClientLinks) != null)
            {
                return;
            }

            var table = new Table(context.Database, ErmTableNames.ClientLinks.Name, ErmTableNames.ClientLinks.Schema);

            table.CreateField(Id, DataType.BigInt, false);

            table.CreateField(MasterClientId, DataType.BigInt, false);
            table.CreateField(ChildClientId, DataType.BigInt, false);

            table.CreateField(CreatedBy, DataType.BigInt, false);
            table.CreateField(ModifiedBy, DataType.BigInt, true);

            table.CreateField(CreatedOn, DataType.DateTime2(2), false);
            table.CreateField(ModifiedOn, DataType.DateTime2(2), true);

            table.CreateField(IsDeleted, DataType.Bit, false);

            table.Create();

            table.CreatePrimaryKey();

            CreateForeignKey(table, MasterClientId, ErmTableNames.Clients, Id);
            CreateForeignKey(table, ChildClientId, ErmTableNames.Clients, Id);
        }

        private static void CreateForeignKey(Table table, string fieldName, SchemaQualifiedObjectName referencedTable, string referencedFieldName)
        {
            var foreignKey = new ForeignKey(table, "FK_" + table.Name + "_" + fieldName + "_" + referencedTable.Name);
            var foreignKeyColumn = new ForeignKeyColumn(foreignKey, fieldName, referencedFieldName);
            foreignKey.Columns.Add(foreignKeyColumn);
            foreignKey.ReferencedTable = referencedTable.Name;
            foreignKey.ReferencedTableSchema = referencedTable.Schema;
            foreignKey.Create();
        }
    }
}
