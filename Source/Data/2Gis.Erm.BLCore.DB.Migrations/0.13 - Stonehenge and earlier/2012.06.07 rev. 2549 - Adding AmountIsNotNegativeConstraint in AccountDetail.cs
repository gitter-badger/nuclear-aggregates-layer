using System;
using System.Text;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(2549, "Добавление ограничения на неотрицательное значение Amount в AccountDetail")]
    public class Migration2549 : TransactedMigration
    {

        #region SQL Statements

        private const string CheckOperationTypeStatement =
            @"
SELECT TOP(1) 1
FROM [Billing].[AccountDetails] AD
JOIN [Billing].OperationTypes OT ON AD.OperationTypeId = OT.Id
WHERE Amount < 0 AND OT.IsPlus = 1;";

        private const string UpdateAmountStatement =
            @"
UPDATE AD
SET	AD.Amount = ABS(AD.Amount)
FROM Billing.AccountDetails AD
WHERE AD.Amount < 0";

        #endregion

        protected override void ApplyOverride(IMigrationContext context)
        {
            // Проверяем данные на корректность
            var checkResult = context.Connection.ExecuteScalar(CheckOperationTypeStatement);
            if (checkResult != null)
            {
                throw new NotSupportedException(
                    "В БД существуют операции по лицевому счету с отрицательной суммой и типом операции, где IsPlus = true");
            }

            var sb = new StringBuilder();

            // Обновляем AccountDetails.Amount на значение по модулю, если оно отрицательное
            sb.Append(UpdateAmountStatement + Environment.NewLine);

            // Добавляем ограничение на отрицательное значение Amount
            const string columnName = "Amount";
            const string constraintName = "CK_Amount_NotNegative";
            var table = context.Database.GetTable(ErmTableNames.AccountDetails);
            if (!table.Checks.Contains(constraintName))
            {
                sb.AppendNotNegaiveConstraintWithDataUpdate(ErmTableNames.AccountDetails, columnName, constraintName);
            }
            context.Connection.ExecuteNonQuery(sb.ToString());
        }
    }
}
