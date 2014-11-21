using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(2505, "Добавление поля 'Макс. количество символов в слове' в ЭШРМ.")]
    public class Migration2505 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.AdvertisementElementTemplates);

            const String columnName = "MaxSymbolsInWord";
            if(!table.Columns.Contains(columnName))
            {
                var newColumnDef = new InsertedColumnDefinition
                    (4, x => new Column(x, columnName, DataType.TinyInt) {Nullable = true});

                EntityCopyHelper.CopyAndInsertColumns(context.Database, table,
                    new List<InsertedColumnDefinition>() { newColumnDef });
            }
        }
    }
}
