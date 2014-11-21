using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5980, "Переименование колонки BannerFileId")]
    public sealed class Migration5980 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.Themes.Name, ErmTableNames.Themes.Schema];
            if (table == null)
                throw new Exception(String.Format("Таблица {0} не найдена", table.Name));

            var column = table.Columns["BannerFileId"];
            if (column == null)
                return;

            column.Rename("FileId");
        }
   }
}