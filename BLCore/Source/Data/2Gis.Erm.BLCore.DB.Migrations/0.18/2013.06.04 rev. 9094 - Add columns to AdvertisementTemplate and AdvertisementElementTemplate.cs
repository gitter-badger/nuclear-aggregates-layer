using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(9094, "Добавлены колонки для заглушек в таблицу AdvertisementTemplate")]
    public sealed class Migration9094 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AddColumnsToAdvertisementTemplateTable(context);
            SetIsPublishedToFalse(context);
            SetIsPublishedNullable(context.Database.GetTable(ErmTableNames.AdvertisementTemplates));
        }

        private static void AddColumnsToAdvertisementTemplateTable(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.AdvertisementTemplates);

            if (table.Columns.Contains("DummyAdvertisementId"))
            {
                return;
            }

            var columnsToInsert = new[]
                {
                    new InsertedColumnDefinition(1,
                                                 x =>
                                                     {
                                                         var c = new Column(x, "DummyAdvertisementId", DataType.BigInt) { Nullable = true };
                                                         return c;
                                                     }),
                    new InsertedColumnDefinition(3,
                                                 x =>
                                                     {
                                                         var c = new Column(x, "IsPublished", DataType.Bit) { Nullable = true };
                                                         return c;
                                                     })
                };
            EntityCopyHelper.CopyAndInsertColumns(context.Database, table, columnsToInsert);
        }

        private static void SetIsPublishedToFalse(IMigrationContext context)
        {
            const string query = "UPDATE Billing.AdvertisementTemplates Set IsPublished = 0";
            context.Connection.ExecuteNonQuery(query);
        }

        private static void SetIsPublishedNullable(Table table)
        {
            var column = table.Columns["IsPublished"];
            if (!column.Nullable)
            {
                return;
            }

            column.AddDefaultConstraint().Text = "0";
            column.Nullable = false;
            column.Alter();
        }
    }
}
