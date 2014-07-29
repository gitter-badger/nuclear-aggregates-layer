using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(21236, "Заполнение колонки OperationEntities в таблице [Shared].[PerformedBusinessOperations], для ранее имевшихся записей, используя значения Descriptor", "i.maslennikov")]
    internal sealed class Migration21236 : TransactedMigration
    {
        private const string TargetColumnName = "OperationEntities";
        private readonly SchemaQualifiedObjectName _targetTableName = ErmTableNames.PerformedBusinessOperations;

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(Resources._21236_Fill_OperationsEntities_From_Descriptor);

            context.Database
                   .GetTable(_targetTableName)
                   .CreateIndex(false, "Operation", TargetColumnName);
        }
    }
}
