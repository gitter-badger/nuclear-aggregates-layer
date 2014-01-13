using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5191, "Добавлена колонка в Integration.CardRelations")]
    public sealed class Migration5191 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(new SchemaQualifiedObjectName("Integration", "CardRelations"));

            if (!table.Columns.Contains("Code"))
            {
                const string clearCardRelations =
                @"DELETE FROM  Integration.CardRelations"; //Инфораша должна перевыгрузить для нас данные

                context.Connection.ExecuteNonQuery(clearCardRelations);

                var codeColumn = new Column(table, "Code", DataType.BigInt) { Nullable = false };

                table.Columns.Add(codeColumn);
                table.Alter();
            }
        }
    }
}
