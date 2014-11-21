using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(8639, "Удаляем неиспользуемые хранимки")]
    public sealed class Migration8639 : TransactedMigration
    {
        private const string SqlStatement = @"
IF OBJECT_ID('{0}') IS NOT NULL
DROP PROC {0}	
GO";

        private readonly string[] _spsToDelete = new[]
            {
                "[Billing].[CheckOrdersAD]",
                "[Billing].[CheckOrdersAssociatedPositions]",
                "[Billing].[CheckOrdersDeniedPositions]",
                "[Security].[svcGetEntitySharings]",
                "[Security].[svcGetSharedEntities]",
                "[Security].[svcGetUserCodes]",
                "[Security].[svcGrantEntityAccess]",
                "[Security].[svcReplaceEntitySharings]",
                "[Security].[svcRevokeEntityAccess]"
            };

        protected override void ApplyOverride(IMigrationContext context)
        {
            foreach (var sps in _spsToDelete)
            {
                context.Connection.ExecuteNonQuery(string.Format(SqlStatement, sps));
            }
        }
    }
}
