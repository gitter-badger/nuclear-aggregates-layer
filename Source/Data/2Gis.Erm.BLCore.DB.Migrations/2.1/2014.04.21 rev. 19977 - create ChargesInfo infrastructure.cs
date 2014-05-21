using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations
{
    [Migration(19977, "Создание таблицы Billing.Charges", "a.tukaev")]
    public class Migration19977 : TransactedMigration
    {
        private const string Id = "Id";
        private const string OrderPositionId = "OrderPositionId";
        private const string ProjectId = "ProjectId";
        private const string PositionId = "PositionId";
        private const string PeriodStartDate = "PeriodStartDate";
        private const string PeriodEndDate = "PeriodEndDate";
        private const string SessionId = "SessionId";
        private const string Status = "Status";
        private const string Message = "Message";
        private const string Comment = "Comment";

        protected override void ApplyOverride(IMigrationContext context)
        {
            CreateCharges(context);

            CreateChargesHistory(context);
        }

        private static void CreateCharges(IMigrationContext context)
        {
            if (context.Database.GetTable(ErmTableNames.Charges) != null)
            {
                return;
            }

            var table = new Table(context.Database, ErmTableNames.Charges.Name, ErmTableNames.Charges.Schema);

            table.CreateField(Id, DataType.BigInt, false);
            table.CreateField(OrderPositionId, DataType.BigInt, false);
            table.CreateField(ProjectId, DataType.BigInt, false);
            table.CreateField(PositionId, DataType.BigInt, false);
            table.CreateField(PeriodStartDate, DataType.DateTime2(2), false);
            table.CreateField(PeriodEndDate, DataType.DateTime2(2), false);
            table.CreateField(SessionId, DataType.UniqueIdentifier, false);
            table.CreateAuditableEntityColumns();

            table.Create();

            table.CreatePrimaryKey();
            table.CreateIndex(ProjectId, PeriodStartDate, PeriodEndDate);
            table.CreateIndex(OrderPositionId, PeriodStartDate, PeriodEndDate);
        }

        private static void CreateChargesHistory(IMigrationContext context)
        {
            if (context.Database.GetTable(ErmTableNames.ChargesHistory) != null)
            {
                return;
            }

            var table = new Table(context.Database, ErmTableNames.ChargesHistory.Name, ErmTableNames.ChargesHistory.Schema);

            table.CreateField(Id, DataType.BigInt, false);
            table.CreateField(ProjectId, DataType.BigInt, false);
            table.CreateField(PeriodStartDate, DataType.DateTime2(2), false);
            table.CreateField(PeriodEndDate, DataType.DateTime2(2), false);
            table.CreateField(Message, DataType.Xml(string.Empty), false);
            table.CreateField(Status, DataType.Int, false);
            table.CreateField(Comment, DataType.NVarCharMax, true);
            table.CreateField(SessionId, DataType.UniqueIdentifier, false);
            table.CreateAuditableEntityColumns();

            table.Create();

            table.CreatePrimaryKey();
            table.CreateIndex(SessionId, Status);
        }
    }
}