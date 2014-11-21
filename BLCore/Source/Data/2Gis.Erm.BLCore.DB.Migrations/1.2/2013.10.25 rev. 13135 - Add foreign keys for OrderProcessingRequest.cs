using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._2
{
    [Migration(13135, "Добавляем внешние ключи для OrderProcessingRequests")]
    public class Migration13135 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.OrderProcessingRequests);

            if (table == null)
            {
                return;
            }

            CreateFk(table, "FK_OrderProcessingRequests_BaseOrders", "BaseOrderId", ErmTableNames.Orders);
            CreateFk(table, "FK_OrderProcessingRequests_RenewedOrders", "RenewedOrderId", ErmTableNames.Orders);
            CreateFk(table, "FK_OrderProcessingRequests_SourceOrganizationUnits", "SourceOrganizationUnitId", ErmTableNames.OrganizationUnits);
            CreateFk(table, "FK_OrderProcessingRequests_Firms", "FirmId", ErmTableNames.Firms);
            CreateFk(table, "FK_OrderProcessingRequests_LegalPersonProfiles", "LegalPersonProfileId", ErmTableNames.LegalPersonProfiles);
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
