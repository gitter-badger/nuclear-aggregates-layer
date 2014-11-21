using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(2490, "Обновление поля 'Обращение' в базе ERM")]
    public class Migration2490 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            String script =
                String.Format(
                    @"UPDATE {0} SET Salutation = 

(CASE
WHEN GenderCode = 1
THEN 'Уважаемый'
WHEN GenderCode = 2
THEN 'Уважаемая'
ELSE
	NULL
	END
);", ErmTableNames.Contacts);
            context.Connection.ExecuteNonQuery(script);
        }
    }
}