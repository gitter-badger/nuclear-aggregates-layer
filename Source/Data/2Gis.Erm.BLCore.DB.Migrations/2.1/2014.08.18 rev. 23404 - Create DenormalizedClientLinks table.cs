using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(23404, "Создание таблицы Billing.DenormalizedClientLinks", "y.baranikhin")]
    public class Migration23404 : TransactedMigration
    {
        private const string Id = "Id";
        private const string MasterClientId = "MasterClientId";
        private const string ChildClientId = "ChildClientId";
        private const string IsLinkedDirectly = "IsLinkedDirectly";

        protected override void ApplyOverride(IMigrationContext context)
        {
            if (context.Database.GetTable(ErmTableNames.DenormalizedClientLinks) != null)
            {
                return;
            }

            var table = new Table(context.Database, ErmTableNames.DenormalizedClientLinks.Name, ErmTableNames.DenormalizedClientLinks.Schema);

            table.CreateField(Id, DataType.BigInt, false);

            table.CreateField(MasterClientId, DataType.BigInt, false);
            table.CreateField(ChildClientId, DataType.BigInt, false);
            table.CreateField(IsLinkedDirectly, DataType.Bit, false);

            table.CreateAuditableEntityColumns();

            table.Create();

            table.CreatePrimaryKey();
            table.CreateForeignKey(MasterClientId, ErmTableNames.Clients, Id, string.Format("FK_{0}_{1}_{2}", table.Name, MasterClientId, ErmTableNames.Clients.Name));
            table.CreateForeignKey(ChildClientId, ErmTableNames.Clients, Id, string.Format("FK_{0}_{1}_{2}", table.Name, ChildClientId, ErmTableNames.Clients.Name));
            table.CreateUniqueIndex(MasterClientId, ChildClientId);
        }
    }
}