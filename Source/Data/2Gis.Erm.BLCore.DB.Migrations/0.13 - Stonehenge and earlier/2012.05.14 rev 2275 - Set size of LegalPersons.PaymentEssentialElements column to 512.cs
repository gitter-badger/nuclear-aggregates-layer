using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(2275, "Размер поля 'Платёжные реквизиты' увеличен с 200 до 512.")]
    public class Migration2275 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var legalPersonsTable = context.Database.GetTable(ErmTableNames.LegalPersons);
            var column = legalPersonsTable.Columns["PaymentEssentialElements"];
            if(column == null)
            {
                throw new InvalidOperationException("Не найдена колонка 'PaymentEssentialElements'");
            }

            if(column.DataType.MaximumLength != 512)
            {
                column.DataType.MaximumLength = 512;
                column.Alter();
            }
        }
    }
}
