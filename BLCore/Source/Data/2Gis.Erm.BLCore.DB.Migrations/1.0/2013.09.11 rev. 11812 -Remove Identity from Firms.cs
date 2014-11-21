using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(11812, "Отключаем автоинкремент в таблице Firms")]
    public class Migration11812 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.Firms);
            if (!table.Columns["Id"].Identity)
            {
                return;
            }

            try
            {
                EntityCopyHelper.RemoveIdentity(context.Database, table);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("While processing {0}", table.Name), e);
            }
        }
    }
}
