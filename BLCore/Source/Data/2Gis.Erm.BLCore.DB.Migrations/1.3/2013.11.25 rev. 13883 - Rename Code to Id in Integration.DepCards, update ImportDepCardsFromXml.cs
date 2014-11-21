using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.DB.Migration.Impl._1_3
{
    [Migration(13833, "Code -> Id в Integration.DepCards, обновляем Integration.ImportDepCardsFromXml", "a.tukaev")]
    public class Migration13883 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.DepCards);
            if (table.Columns["Id"] != null)
            {
                return;
            }

            var codeColumn = table.Columns["Code"];
            codeColumn.Rename("Id");

            context.Database.ExecuteNonQuery(Resources.ImportDepCardsFromXml_13833);
        } 
    }
}