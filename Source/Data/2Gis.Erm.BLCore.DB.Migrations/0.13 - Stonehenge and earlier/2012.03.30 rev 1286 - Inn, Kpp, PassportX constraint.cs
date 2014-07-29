using System;
using System.Text;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(1286, "В таблице LegalPerson в 5 колонках ставится NULL при отсутствии значения." + 
        " Добавлено ограничение на неравенство этих полей пустой строке.")]
// ReSharper disable InconsistentNaming
    public class Migration_1286_Inn_Kpp_constraint : TransactedMigration
// ReSharper restore InconsistentNaming
    {
        private static readonly String[] AffectedColumns = new String[] { "Inn", "Kpp", "PassportSeries", "PassportNumber", "PassportIssuedBy"};

        private static String GetConstraintNameForColumn(String columnName)
        {
            return String.Format("CK_{0}_Not_Empty", columnName);
        }

        protected override void ApplyOverride(IMigrationContext context)
        {
            var sb = new StringBuilder();
            var table = context.Database.Tables[ErmTableNames.LegalPersons.Name, ErmTableNames.LegalPersons.Schema];

            foreach (String column in AffectedColumns)
            {
                var constraintName = GetConstraintNameForColumn(column);
                if (!table.Checks.Contains(constraintName))
                {
                    sb.AppendNotEmptyConstraintWithDataUpdate(ErmTableNames.LegalPersons, column, constraintName);
                }
            }

            context.Connection.ExecuteNonQuery(sb.ToString());
        }

        protected override void RevertOverride(IMigrationContext context)
        {
            var sb = new StringBuilder();
            var table = context.Database.Tables[ErmTableNames.LegalPersons.Name, ErmTableNames.LegalPersons.Schema];

            foreach (String column in AffectedColumns)
            {
                var constraintName = GetConstraintNameForColumn(column);
                if (!table.Checks.Contains(constraintName))
                {
                    sb.AppendSqlDropConstraint(ErmTableNames.LegalPersons, constraintName);
                }
            }

            context.Connection.ExecuteNonQuery(sb.ToString());
        }
    }
}