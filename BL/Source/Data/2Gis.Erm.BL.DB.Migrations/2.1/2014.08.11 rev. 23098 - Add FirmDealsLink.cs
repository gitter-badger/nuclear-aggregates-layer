using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(23098, "создаем сущность связи фирмы и сделки", "y.baranikhin")]
    public class Migration23098 : TransactedMigration
    {
        private const string Id = "Id";
        private const string FirmId = "FirmId";
        private const string DealId = "DealId";

        protected override void ApplyOverride(IMigrationContext context)
        {
            if (context.Database.GetTable(ErmTableNames.FirmDeals) != null)
            {
                return;
            }

            var table = new Table(context.Database, ErmTableNames.FirmDeals.Name, ErmTableNames.FirmDeals.Schema);

            table.CreateField(Id, DataType.BigInt, false);

            table.CreateField(FirmId, DataType.BigInt, false);
            table.CreateField(DealId, DataType.BigInt, false);

            table.CreateAuditableEntityColumns();
            table.CreateDeleteableEntityColumn();

            table.Create();

            table.CreatePrimaryKey();
            table.CreateForeignKey(FirmId, ErmTableNames.Firms, Id);
            table.CreateForeignKey(DealId, ErmTableNames.Deals, Id);
        }
    }
}