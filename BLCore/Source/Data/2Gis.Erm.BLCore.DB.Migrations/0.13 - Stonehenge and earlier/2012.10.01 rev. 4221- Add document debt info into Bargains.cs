using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4221, "Добавлены две колонки в таблицу Bargains: наличие документов и комментарий к ним")]
    public class Migration4221 : TransactedMigration
    {
        private const String HasDocumentsDebt = "HasDocumentsDebt";
        private const String DocumentsComment = "DocumentsComment";


        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.Bargains.Name, ErmTableNames.Bargains.Schema];

            if(!table.Columns.Contains(HasDocumentsDebt))
            {
                var hasDocumentsDebt = new Column(table, HasDocumentsDebt, DataType.TinyInt) {Nullable = false};
                hasDocumentsDebt.AddDefaultConstraint("DF_Bargains_HasDocumentsDebt");
                hasDocumentsDebt.DefaultConstraint.Text = "1";

                table.Columns.Add(hasDocumentsDebt);
            }

            if(!table.Columns.Contains(DocumentsComment))
            {
                var documentsComment = new Column(table, DocumentsComment, DataType.NVarChar(300)) { Nullable = true };

                table.Columns.Add(documentsComment);
            }
            
            table.Alter();
        }
    }
}