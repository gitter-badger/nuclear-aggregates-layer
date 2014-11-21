using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5549, "Удаление использования BusinessDirectory.Territories.IsDeleted из вьюхи [BusinessDirectory].[vwTerritories]")]
    public sealed class Migration5549 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            #region Текст запроса
            const string query = @"
ALTER VIEW [BusinessDirectory].[vwTerritories]
AS
SELECT     Id, Name, OrganizationUnitId, OwnerCode, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn
FROM         BusinessDirectory.Territories
WHERE     (IsActive = 1)";
            #endregion

            context.Connection.ExecuteNonQuery(query);
        }
    }
}
