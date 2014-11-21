using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6808, "Для городов на дгпп признак пустого адреса неактуален, выставляем признак в - не пустой")]
    public sealed class Migration6808 : TransactedMigration
    {
        #region Текст запроса
        private const string Query = @"  UPDATE BusinessDirectory.FirmAddresses SET IsLocatedOnTheMap = 1 WHERE FirmId IN (
  SELECT Id FROM BusinessDirectory.Firms WHERE OrganizationUnitId in (
  SELECT Id FROM [Billing].[OrganizationUnits] WHERE InfoRussiaLaunchDate Is Null))";

        #endregion

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Query);
        }
    }
}
