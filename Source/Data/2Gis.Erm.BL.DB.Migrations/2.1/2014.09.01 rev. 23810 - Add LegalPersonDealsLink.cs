using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(23810, "создаем сущность связи юр. лица и сделки", "y.baranikhin")]
    public class Migration23810 : TransactedMigration
    {
        private const string Id = "Id";
        private const string LegalPersonId = "LegalPersonId";
        private const string DealId = "DealId";
        private const string IsMain = "IsMain";

        protected override void ApplyOverride(IMigrationContext context)
        {
            if (context.Database.GetTable(ErmTableNames.LegalPersonDeals) != null)
            {
                return;
            }

            var table = new Table(context.Database, ErmTableNames.LegalPersonDeals.Name, ErmTableNames.LegalPersonDeals.Schema);

            table.CreateField(Id, DataType.BigInt, false);

            table.CreateField(LegalPersonId, DataType.BigInt, false);
            table.CreateField(DealId, DataType.BigInt, false);
            table.CreateField(IsMain, DataType.Bit, false);

            table.CreateAuditableEntityColumns();
            table.CreateDeleteableEntityColumn();

            table.Create();

            table.CreatePrimaryKey();
            table.CreateForeignKey(LegalPersonId, ErmTableNames.LegalPersons, Id);
            table.CreateForeignKey(DealId, ErmTableNames.Deals, Id);
        }
    }
}