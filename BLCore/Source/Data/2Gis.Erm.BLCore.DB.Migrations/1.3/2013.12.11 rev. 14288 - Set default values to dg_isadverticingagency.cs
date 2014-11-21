using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1._3
{
    [Migration(14288, "выставляем первичные значения признака рекламное агентство", "y.baranihin")]
    public sealed class Migration14288 : TransactedMigration, INonDefaultDatabaseMigration
    {
        private const string UpdateStatement = @"
		Update [dbo].[AccountExtensionBase] Set [dg_isadvertisingagency] = 0 WHERE [dg_isadvertisingagency] is NULL";

        public ErmConnectionStringKey ConnectionStringKey
        {
            get { return ErmConnectionStringKey.CrmDatabase; }
        }

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(UpdateStatement);
        }
    }
}