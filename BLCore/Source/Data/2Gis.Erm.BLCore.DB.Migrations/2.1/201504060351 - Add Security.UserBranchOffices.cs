using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201504060351, "Создаем таблицу Security.UserBranchOffices", "y.baranihin")]
    public class Migration201504060351 : TransactedMigration
    {
        private const string Id = "Id";
        private const string UserId = "UserId";
        private const string BranchOfficeId = "BranchOfficeId";

        protected override void ApplyOverride(IMigrationContext context)
        {
            var targetTableDescription = ErmTableNames.UserBranchOffices;
            var table = context.Database.Tables[targetTableDescription.Name, targetTableDescription.Schema];
            if (table != null)
            {
                return;
            }

            table = new Table(context.Database, targetTableDescription.Name, targetTableDescription.Schema);

            table.CreateField(Id, DataType.BigInt, false);
            table.CreateField(UserId, DataType.BigInt, false);
            table.CreateField(BranchOfficeId, DataType.BigInt, false);
            table.CreateAuditableEntityColumns();
            table.CreateTimestampColumn();

            table.Create();
            table.CreatePrimaryKey(Id);
            table.CreateForeignKey(UserId, ErmTableNames.Users, Id);
            table.CreateForeignKey(BranchOfficeId, ErmTableNames.BranchOffices, Id);
        }
    }
}