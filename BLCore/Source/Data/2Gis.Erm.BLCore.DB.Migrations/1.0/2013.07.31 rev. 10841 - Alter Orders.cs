using System.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(10841, "Делаем FirmId обязательным в таблице Orders; удаляем заказы с незаполненным FirmId и соответствующие им записи в других таблицах")]
    public class Migration10841 : TransactedMigration
    {
        #region Statement

        private const string DeleteStatement = @"
delete Billing.ReleasesWithdrawalsPositions from Billing.ReleasesWithdrawalsPositions rwp
       join Billing.ReleasesWithdrawals rw on rw.Id = rwp.ReleasesWithdrawalId
	   join Billing.OrderPositions op on op.Id = rw.OrderPositionId
	   join Billing.Orders o on o.Id = op.OrderId
	   where o.FirmId is null

delete Billing.ReleasesWithdrawals from Billing.ReleasesWithdrawals rw 
	   join Billing.OrderPositions op on op.Id = rw.OrderPositionId
	   join Billing.Orders o on o.Id = op.OrderId
	   where o.FirmId is null

delete Billing.OrderPositionAdvertisement from Billing.OrderPositionAdvertisement opa 
	   join Billing.OrderPositions op on op.Id = opa.OrderPositionId
	   join Billing.Orders o on o.Id = op.OrderId
	   where o.FirmId is null

delete Billing.OrderPositions from Billing.OrderPositions op 
	   join Billing.Orders o on o.Id = op.OrderId
	   where o.FirmId is null

delete Billing.Bills from Billing.Bills b 
	   join Billing.Orders o on o.Id = b.OrderId
	   where o.FirmId is null

delete Billing.OrdersRegionalAdvertisingSharings from Billing.OrdersRegionalAdvertisingSharings oras 
	   join Billing.Orders o on o.Id = oras.OrderId
	   where o.FirmId is null

delete Billing.OrderReleaseTotals from Billing.OrderReleaseTotals ort 
	   join Billing.Orders o on o.Id = ort.OrderId
	   where o.FirmId is null

delete Billing.OrderValidationResults from Billing.OrderValidationResults ovr 
	   join Billing.Orders o on o.Id = ovr.OrderId
	   where o.FirmId is null

delete Billing.OrderFiles from Billing.OrderFiles orf 
	   join Billing.Orders o on o.Id = orf.OrderId
	   where o.FirmId is null

delete from Billing.Orders Where FirmId is null
";

        #endregion
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(DeleteStatement);

            var ordersTable = context.Database.GetTable(ErmTableNames.Orders);

            var firmIdColumn = ordersTable.Columns["FirmId"];

            var indexesToDrop = ordersTable.Indexes.Cast<Index>().Where(x => x.IndexedColumns.Contains(firmIdColumn.Name)).ToArray();
            var indexesToCreate = indexesToDrop.Select(EntityCopyHelper.CopyIndex).ToArray();

            foreach (var index in indexesToDrop)
            {
                index.Drop();
            }

            firmIdColumn.Nullable = false;
            firmIdColumn.Alter();

            foreach (var index in indexesToCreate)
            {
                index.Create();
            }
        }
    }
}