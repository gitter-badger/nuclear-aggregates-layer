using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(7525, "Подготовка к отложенной репликации")]
    public sealed class Migration7525 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AddCrmReplicationInfoTable(context);
            AddCrmReplicationDetailsTable(context);
            RefreshReplicationSPs(context);
            InsertReplicateInfo(context);
            AddDeferredReplicationSP(context);
            DropReplicateFirmsColumn(context);
            AlterIntegrationSPs(context);
            AlterExportOrders(context);
        }

        private void RefreshReplicationSPs(IMigrationContext context)
        {
            var query = Resources._2013_02_13_rev__7055___ReplicateTerritories;
            context.Connection.ExecuteNonQuery(query);

            query = Resources._2013_02_13_rev__7055___ReplicateFirms;
            context.Connection.ExecuteNonQuery(query);

            query = Resources._2013_02_13_rev__7055___ReplicateFirmAddresses;
            context.Connection.ExecuteNonQuery(query);
        }

        private void AddCrmReplicationInfoTable(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.CrmReplicationInfo.Name, ErmTableNames.CrmReplicationInfo.Schema];
            if (table != null)
            {
                return;
            }

            table = new Table(context.Database, ErmTableNames.CrmReplicationInfo.Name, ErmTableNames.CrmReplicationInfo.Schema);

            table.Columns.Add(new Column(table, "Id", DataType.Int) { Nullable = false, Identity = true, IdentitySeed = 1, IdentityIncrement = 1 });
            table.CreateField("Entity", DataType.NVarChar(100), false);
            table.CreateField("LastTimestamp", DataType.Binary(8), false);
            table.CreateField("ModifiedOn", DataType.DateTime2(2), false);

            table.Create();
            table.CreatePrimaryKey();
        }

        private void AddCrmReplicationDetailsTable(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.CrmReplicationDetails.Name, ErmTableNames.CrmReplicationDetails.Schema];
            if (table != null)
            {
                return;
            }

            table = new Table(context.Database, ErmTableNames.CrmReplicationDetails.Name, ErmTableNames.CrmReplicationDetails.Schema);

            table.Columns.Add(new Column(table, "Id", DataType.Int) { Nullable = false, Identity = true, IdentitySeed = 1, IdentityIncrement = 1 });
            table.CreateField("Entity", DataType.NVarChar(100), false);
            table.CreateField("LastTimestamp", DataType.Binary(8), false);
            table.CreateField("ReplicatedCount", DataType.Int, false);
            table.CreateField("CreatedOn", DataType.DateTime2(2), false);

            table.Create();
            table.CreatePrimaryKey();
        }

        private void InsertReplicateInfo(IMigrationContext context)
        {
            var query = Resources._2013_02_13_rev__7055___AddReplicateInfo;
            context.Connection.ExecuteNonQuery(query);
        }

        private void AddDeferredReplicationSP(IMigrationContext context)
        {
            var query = Resources._2013_02_13_rev__7055___ReplicateEntitiesToCrm;
            context.Connection.ExecuteNonQuery(query);
        }

        private void AlterIntegrationSPs(IMigrationContext context)
        {
            var query = Resources._2013_02_13_rev__7055___Alter_Integration_SPs;
            context.Connection.ExecuteNonQuery(query);
        }

        private void AlterExportOrders(IMigrationContext context)
        {
            var query = Resources._2013_02_13_rev__7055___Alter_Export_Orders;
            context.Connection.ExecuteNonQuery(query);
        }

        private void DropReplicateFirmsColumn(IMigrationContext context)
        {
            var table = context.Database.Tables["FirmsForPostIntegrationActivities", ErmSchemas.Integration];

            var column = table.Columns["ReplicateObjects"];
            if (column != null)
            {
                column.Drop();
            }
        }
    }
}
